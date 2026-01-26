using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<ListProductsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProductsAsync(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListProductsQuery(), cancellationToken);
        var data = _mapper.Map<IEnumerable<ListProductsResponse>>(response);

        return Ok(new ApiResponseWithData<IEnumerable<ListProductsResponse>>
        {
            Success = true,
            Message = "Products retrieved successfully",
            Data = data
        });
    }
}
