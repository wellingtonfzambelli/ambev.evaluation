using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public sealed class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<Sale, GetSaleResult>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.SaleStatus.ToString()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount));

        CreateMap<SaleItem, GetSaleItemResult>()
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.PercentageDiscount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));
    }
}