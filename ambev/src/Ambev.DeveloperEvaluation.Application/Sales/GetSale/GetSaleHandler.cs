using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public sealed class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public GetSaleHandler(ISaleRepository saleRepository, IMapper mapper, IDistributedCache cache)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<GetSaleResult> Handle(GetSaleQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cacheKey = SalesCacheKeys.GetById(request.Id);
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedData))
        {
            return JsonSerializer.Deserialize<GetSaleResult>(cachedData) ?? new GetSaleResult();
        }

        if (await _saleRepository.GetByIdWithItemsAsync(request.Id, cancellationToken)
            is var sale && sale is null)
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    nameof(GetSaleQuery.Id),
                    $"Sale with ID {request.Id} not found")
            });

        var result = _mapper.Map<GetSaleResult>(sale);
        result.Id = sale.Id;

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            },
            cancellationToken);

        return result;
    }
}
