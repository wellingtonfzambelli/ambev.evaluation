using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for replacing sale items.
/// </summary>
public sealed class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(ISaleRepository saleRepository, IProductRepository productRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdWithItemsAsync(request.Id, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        var replacementItems = new List<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)>();
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");

            replacementItems.Add((item.ProductId, product.Name, item.Quantity, item.UnitPrice));
        }

        sale.ReplaceItems(replacementItems);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return _mapper.Map<UpdateSaleResult>(sale);
    }
}
