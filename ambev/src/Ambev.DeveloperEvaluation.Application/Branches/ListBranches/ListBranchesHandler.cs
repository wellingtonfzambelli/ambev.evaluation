using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Application.Branches.ListBranches;

public sealed class ListBranchesHandler : IRequestHandler<ListBranchesQuery, IReadOnlyList<ListBranchesResult>>
{
    private const string CacheKey = "branches:all";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public ListBranchesHandler(IBranchRepository branchRepository, IMapper mapper, IDistributedCache cache)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ListBranchesResult>> Handle
    (
        ListBranchesQuery request,
        CancellationToken cancellationToken
    )
    {
        var cachedData = await _cache.GetStringAsync(CacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedData))
        {
            return JsonSerializer.Deserialize<List<ListBranchesResult>>(cachedData)
                ?? new List<ListBranchesResult>();
        }

        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        var data = _mapper.Map<IReadOnlyList<ListBranchesResult>>(branches.ToList());

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