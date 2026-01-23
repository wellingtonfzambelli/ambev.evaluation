using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for retrieving a sale by ID.
/// </summary>
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

        var sale = await _saleRepository.GetByIdWithItemsAsync(request.Id, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        var result = _mapper.Map<GetSaleResult>(sale);

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
