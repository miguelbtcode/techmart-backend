using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TechMart.Product.Application.Features.Inventory.Commands.ReserveStock;
using TechMart.Product.Application.Features.Inventory.Commands.UpdateStock;
using TechMart.Product.Application.Features.Inventory.Queries.GetInventoryByProduct;
using TechMart.Product.Application.Features.Inventory.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[EnableRateLimiting("ApiRateLimit")]
public class InventoryController(IMediator mediator) : BaseApiController
{
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<InventoryVm>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<InventoryVm>>> GetInventoryByProduct(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInventoryByProductQuery(productId);
        var result = await mediator.Send(query, cancellationToken);
        
        return HandleResult(result);
    }
    
    [HttpPatch("product/{productId:guid}/stock")]
    [Authorize(Roles = "Admin,InventoryManager")]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(ApiResponse), 204)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> UpdateStock(
        Guid productId,
        [FromBody] UpdateStockRequest request,
        CancellationToken cancellationToken = default)
    {
        if (productId != request.ProductId)
        {
            return BadRequest(ApiResponse.FailureResponse("ID mismatch",
                ["URL product ID does not match body product ID"], 
                HttpContext.TraceIdentifier));
        }

        var command = new UpdateStockCommand(
            request.ProductId,
            request.QuantityAdjustment,
            request.Reason,
            User.Identity?.Name);

        var result = await mediator.Send(command, cancellationToken);

        return HandleUpdatedResult(result);
    }
    
    [HttpPost("product/{productId:guid}/reserve")]
    [Authorize(Roles = "Admin,OrderManager,InventoryManager")]
    [EnableRateLimiting("AdminRateLimit")]
    [ProducesResponseType(typeof(ApiResponse), 204)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<ActionResult<ApiResponse>> ReserveStock(
        Guid productId,
        [FromBody] ReserveStockRequest request,
        CancellationToken cancellationToken = default)
    {
        if (productId != request.ProductId)
        {
            return BadRequest(ApiResponse.FailureResponse("ID mismatch",
                ["URL product ID does not match body product ID"], 
                HttpContext.TraceIdentifier));
        }

        var command = new ReserveStockCommand(
            request.ProductId,
            request.Quantity,
            request.ReservationId,
            User.Identity?.Name);

        var result = await mediator.Send(command, cancellationToken);

        return HandleUpdatedResult(result);
    }
}

// Request models for the API
public class UpdateStockRequest
{
    public Guid ProductId { get; set; }
    public int QuantityAdjustment { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ReserveStockRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? ReservationId { get; set; }
}