using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public sealed class CancelSaleHandler : IRequestHandler<CancelSaleCommand>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IDistributedCache _cache;
    private readonly ISaleCancelledPublisher _publisher;

    public CancelSaleHandler(
        ISaleRepository saleRepository,
        IDistributedCache cache,
        ISaleCancelledPublisher publisher)
    {
        _saleRepository = saleRepository;
        _cache = cache;
        _publisher = publisher;
    }

    public async Task Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale is null)
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(CancelSaleCommand.Id),
                    $"Sale with ID {request.Id} not found")
            });

        if (!sale.CancelStatus())
            throw new InvalidOperationException("Sale is already canceled");

        await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _cache.RemoveAsync(SalesCacheKeys.All, cancellationToken);
        await _cache.RemoveAsync(SalesCacheKeys.GetById(sale.Id), cancellationToken);
        await _publisher.PublishAsync(sale.Id, cancellationToken);
    }
}
