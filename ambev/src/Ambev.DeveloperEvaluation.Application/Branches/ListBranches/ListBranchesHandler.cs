using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.ListBranches;

public sealed class ListBranchesHandler : IRequestHandler<ListBranchesQuery, IReadOnlyList<ListBranchesResult>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public ListBranchesHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ListBranchesResult>> Handle
    (
        ListBranchesQuery request,
        CancellationToken cancellationToken
    )
    {
        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ListBranchesResult>>(branches.ToList());
    }
}