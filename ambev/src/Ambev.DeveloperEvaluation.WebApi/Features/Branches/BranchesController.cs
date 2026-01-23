using Ambev.DeveloperEvaluation.Application.Branches.ListBranches;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.ListBranches;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches;

[ApiController]
[Route("api/[controller]")]
public sealed class BranchesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public BranchesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<ListBranchesResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListBranchesAsync(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListBranchesQuery(), cancellationToken);

        return Ok(new ApiResponseWithData<IEnumerable<ListBranchesResponse>>
        {
            Success = true,
            Message = "Branches retrieved successfully",
            Data = _mapper.Map<IEnumerable<ListBranchesResponse>>(response)
        });
    }
}