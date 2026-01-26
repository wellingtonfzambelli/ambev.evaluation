using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Profile for mapping GetSale feature requests to queries and responses.
/// </summary>
public sealed class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<Guid, GetSaleQuery>()
            .ConstructUsing(id => new GetSaleQuery(id));

        CreateMap<GetSaleResult, GetSaleResponse>()
            .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id));

        CreateMap<GetSaleItemResult, GetSaleItemResponse>();
    }
}
