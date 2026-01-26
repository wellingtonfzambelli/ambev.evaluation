using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public sealed class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ISaleUpdatedPublisher _publisher;

    public UpdateSaleHandler
    (
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IMapper mapper,
        IDistributedCache cache,
        ISaleUpdatedPublisher publisher
    )
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _cache = cache;
        _publisher = publisher;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (await _saleRepository.GetByIdWithItemsAsync(request.Id, cancellationToken)
            is var sale && sale is null)
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(UpdateSaleCommand.Id),
                    $"Sale with ID {request.Id} not found")
            });

        if (sale.SaleStatus == SaleStatus.Canceled)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(Sale.SaleStatus),
                    "Cannot update items for a canceled sale.")
            });
        }

        var replacementItems = new List<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)>();
        foreach (var item in request.Items)
        {
            if (await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                is var product && product is null)
                throw new ValidationException(new[]
                {
                    new ValidationFailure(
                        nameof(UpdateSaleItemCommand.ProductId),
                        $"Product with ID {item.ProductId} not found")
                });

            replacementItems.Add((item.ProductId, product.Name, item.Quantity, item.UnitPrice));
        }

        sale.ReplaceItems(replacementItems);

        await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _cache.RemoveAsync(SalesCacheKeys.All, cancellationToken);
        await _cache.RemoveAsync(SalesCacheKeys.GetById(sale.Id), cancellationToken);
        await _publisher.PublishAsync(sale.Id, cancellationToken);

        return _mapper.Map<UpdateSaleResult>(sale);
    }
}