using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Branches.ListBranches;

public sealed class ListBranchesProfile : Profile
{
    public ListBranchesProfile()
    {
        CreateMap<Branch, ListBranchesResult>();
    }
}
