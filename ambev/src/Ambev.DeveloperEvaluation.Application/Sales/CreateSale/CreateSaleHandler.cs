using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public sealed class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public CreateSaleHandler
    (
       ISaleRepository saleRepository,
       IMapper mapper
    )
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<CreateSaleResult> Handle
    (
        CreateSaleCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var validationResult = new CreateSaleCommandValidator()
                .Validate(command);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var sale = new Sale
            (
                command.SaleNumber,
                DateTime.UtcNow,
                command.CustomerId,
                command.BranchId
            );

            foreach (var item in command.Items)
            {
                sale.AddItem
                (
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice
                );
            }

            await _saleRepository.CreateAsync(sale, cancellationToken);

            return _mapper.Map<CreateSaleResult>(sale);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}