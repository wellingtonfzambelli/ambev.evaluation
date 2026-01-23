using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public sealed class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public CreateSaleHandler
    (
       ISaleRepository saleRepository,
       IUserRepository userRepository,
       IBranchRepository branchRepository,
       IProductRepository productRepository,
       IMapper mapper,
       IDistributedCache cache
    )
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _branchRepository = branchRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<CreateSaleResult> Handle
    (
        CreateSaleCommand command,
        CancellationToken cancellationToken
    )
    {
        var idempotencyKey = SalesCacheKeys.Idempotency(command.SaleNumber);
        if (!string.IsNullOrWhiteSpace(await _cache.GetStringAsync(idempotencyKey, cancellationToken)))
            throw new InvalidOperationException("Duplicate request is not allowed. Please wait 5 minutes.");

        var validationResult = new CreateSaleCommandValidator().Validate(command);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (await _userRepository.GetByIdAsync(command.CustomerId, cancellationToken)
            is var customer && customer is null)
            throw new KeyNotFoundException($"Customer with ID {command.CustomerId} not found");

        if (await _branchRepository.GetByIdAsync(command.BranchId, cancellationToken)
            is var branch && branch is null)
            throw new KeyNotFoundException($"Branch with ID {command.BranchId} not found");

        var sale = new Sale
        (
            command.SaleNumber,
            DateTime.UtcNow,
            command.CustomerId,
            customer.Username,
            command.BranchId,
            branch.Name
        );

        foreach (var item in command.Items)
        {
            if (await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                is var product && product is null)
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");

            sale.AddItem
            (
                item.ProductId,
                product.Name,
                item.Quantity,
                item.UnitPrice
            );
        }

        await _cache.SetStringAsync(
            idempotencyKey,
            "1",
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
            cancellationToken);

        await _saleRepository.CreateAsync(sale, cancellationToken);
        await _cache.RemoveAsync(SalesCacheKeys.All, cancellationToken);
        await _cache.RemoveAsync(SalesCacheKeys.GetById(sale.Id), cancellationToken);

        return _mapper.Map<CreateSaleResult>(sale);
    }
}