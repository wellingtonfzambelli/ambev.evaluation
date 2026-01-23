using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public sealed class ListSalesHandler : IRequestHandler<ListSalesQuery, IReadOnlyList<ListSalesResult>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper, IDistributedCache cache)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ListSalesResult>> Handle
    (
        ListSalesQuery request,
        CancellationToken cancellationToken)
    {
        var cachedData = await _cache.GetStringAsync(SalesCacheKeys.All, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedData))
        {
            return JsonSerializer.Deserialize<List<ListSalesResult>>(cachedData)
                ?? new List<ListSalesResult>();
        }

        var sales = await _saleRepository.ListAsync(cancellationToken);
        var data = _mapper.Map<IReadOnlyList<ListSalesResult>>(sales);

        await _cache.SetStringAsync
        (
            SalesCacheKeys.All,
            JsonSerializer.Serialize(data),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            },
            cancellationToken
        );

        return data;
    }
}
