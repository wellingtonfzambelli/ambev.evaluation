using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// AutoMapper profile for sales listing.
/// </summary>
public sealed class ListSalesProfile : Profile
{
    public ListSalesProfile()
    {
        CreateMap<ListSalesResult, ListSalesResponse>()
            .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id));
    }
}
