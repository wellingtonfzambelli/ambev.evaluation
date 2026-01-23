using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public sealed class ListProductsHandler : IRequestHandler<ListProductsQuery, IReadOnlyList<ListProductsResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ListProductsResult>> Handle
    (
        ListProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ListProductsResult>>(products.ToList());
    }
}
