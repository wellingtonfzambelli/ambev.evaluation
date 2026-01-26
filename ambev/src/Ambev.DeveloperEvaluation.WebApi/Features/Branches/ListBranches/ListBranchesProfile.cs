using Ambev.DeveloperEvaluation.Application.Branches.ListBranches;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.ListBranches;

public sealed class ListBranchesProfile : Profile
{
    public ListBranchesProfile()
    {
        CreateMap<ListBranchesResult, ListBranchesResponse>()
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.Id));
    }
}
