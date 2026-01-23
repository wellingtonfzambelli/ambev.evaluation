using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public sealed class ListProductsProfile : Profile
{
    public ListProductsProfile()
    {
        CreateMap<ListProductsResult, ListProductsResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));
    }
}
