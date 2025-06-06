using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMart.Product.Application.Features.Brands.Commands.CreateBrand;
using TechMart.Product.Application.Features.Brands.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new brand
    /// </summary>
    /// <param name="command">Brand creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created brand</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,BrandManager")]
    [ProducesResponseType(typeof(ApiResponse<BrandVm>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<ActionResult<ApiResponse<BrandVm>>> CreateBrand(
        [FromBody] CreateBrandCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                "GetBrand", // This would need to be implemented
                new { id = result.Value.Id }, 
                ApiResponse<BrandVm>.SuccessResponse(result.Value, "Brand created successfully"));
        }

        return result.Error.Type switch
        {
            ErrorType.Conflict => Conflict(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
            ErrorType.Validation => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
            _ => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier))
        };
    }
}