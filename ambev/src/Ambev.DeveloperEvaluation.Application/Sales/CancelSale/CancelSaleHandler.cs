using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public sealed class CancelSaleHandler : IRequestHandler<CancelSaleCommand>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IDistributedCache _cache;

    public CancelSaleHandler(ISaleRepository saleRepository, IDistributedCache cache)
    {
        _saleRepository = saleRepository;
        _cache = cache;
    }

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
        await _cache.RemoveAsync(SalesCacheKeys.All, cancellationToken);
        await _cache.RemoveAsync(SalesCacheKeys.GetById(sale.Id), cancellationToken);
    }
}
