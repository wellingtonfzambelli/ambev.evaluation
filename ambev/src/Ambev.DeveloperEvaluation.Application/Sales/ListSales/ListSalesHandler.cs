using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public sealed class ListSalesHandler : IRequestHandler<ListSalesQuery, IReadOnlyList<ListSalesResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ListSalesResult>> Handle
    (
        ListSalesQuery request,
        CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.ListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ListSalesResult>>(sales);
    }
}
