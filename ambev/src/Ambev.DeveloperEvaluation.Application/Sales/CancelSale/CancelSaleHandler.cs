using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public sealed class CancelSaleHandler : IRequestHandler<CancelSaleCommand>
{
    private readonly ISaleRepository _saleRepository;

    public CancelSaleHandler(ISaleRepository saleRepository) =>
        _saleRepository = saleRepository;

    public async Task Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        if (!sale.CancelStatus())
            throw new InvalidOperationException("Sale is already canceled");

        await _saleRepository.UpdateAsync(sale, cancellationToken);
    }
}
