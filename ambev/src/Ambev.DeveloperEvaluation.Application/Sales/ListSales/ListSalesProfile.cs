using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public sealed class ListSalesProfile : Profile
{
    public ListSalesProfile()
    {
        CreateMap<Sale, ListSalesResult>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.SaleStatus.ToString()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));

        CreateMap<SaleItem, ListSalesItemResult>()
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.PercentageDiscount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));
    }
}
