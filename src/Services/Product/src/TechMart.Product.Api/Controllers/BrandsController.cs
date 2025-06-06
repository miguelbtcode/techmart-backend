using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMart.Product.Application.Features.Brands.Commands.CreateBrand;
using TechMart.Product.Application.Features.Brands.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController(IMediator mediator) : BaseApiController
{
    [HttpPost]
    [Authorize(Roles = "Admin,BrandManager")]
    [ProducesResponseType(typeof(ApiResponse<BrandVm>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<ActionResult<ApiResponse<BrandVm>>> CreateBrand(
        [FromBody] CreateBrandCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        // 🔥 SÚPER LIMPIO - Sin "this."
        return HandleCreatedResult(result, nameof(GetBrand), new { id = result.Value?.Id });
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BrandVm>>> GetBrand(Guid id)
    {
        // var result = await _mediator.Send(new GetBrandQuery(id));
        // return HandleResult(result); // ← Limpio y natural
        throw new NotImplementedException();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteBrand(Guid id)
    {
        // var result = await _mediator.Send(new DeleteBrandCommand(id));
        // return HandleDeletedResult(result); // ← Devuelve 204 No Content
        throw new NotImplementedException();
    }
}