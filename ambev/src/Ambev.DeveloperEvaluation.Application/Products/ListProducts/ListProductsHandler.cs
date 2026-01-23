using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public sealed class ListProductsHandler : IRequestHandler<ListProductsQuery, IReadOnlyList<ListProductsResult>>
{
    private const string CacheKey = "products:all";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public ListProductsHandler(IProductRepository productRepository, IMapper mapper, IDistributedCache cache)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ListProductsResult>> Handle
    (
        ListProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        var cachedData = await _cache.GetStringAsync(CacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedData))
        {
            return JsonSerializer.Deserialize<List<ListProductsResult>>(cachedData)
                ?? new List<ListProductsResult>();
        }

        var products = await _productRepository.GetAllAsync(cancellationToken);
        var data = _mapper.Map<IReadOnlyList<ListProductsResult>>(products.ToList());

        await _cache.SetStringAsync(
            CacheKey,
            JsonSerializer.Serialize(data),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            },
            cancellationToken);

        return data;
    }
}
