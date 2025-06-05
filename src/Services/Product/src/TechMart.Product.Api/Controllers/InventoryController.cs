using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Application.Features.Inventory.Commands.ReserveStock;
using TechMart.Product.Application.Features.Inventory.Commands.UpdateStock;
using TechMart.Product.Application.Features.Inventory.Queries.GetInventoryByProduct;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[EnableRateLimiting("ApiRateLimit")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets inventory information for a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product inventory details</returns>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<InventoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<InventoryDto>>> GetInventoryByProduct(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInventoryByProductQuery(productId);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<InventoryDto>.SuccessResponse(result.Value))
            : NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Updates stock levels for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="command">Stock update command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
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
                new[] { "URL product ID does not match body product ID" }, 
                HttpContext.TraceIdentifier));
        }

        var command = new UpdateStockCommand(
            request.ProductId,
            request.QuantityAdjustment,
            request.Reason,
            User.Identity?.Name);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? NoContent()
            : result.Error.Type switch
            {
                ErrorType.NotFound => NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                ErrorType.Validation => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                _ => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier))
            };
    }

    /// <summary>
    /// Reserves stock for a product (typically used during order processing)
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="request">Stock reservation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
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
                new[] { "URL product ID does not match body product ID" }, 
                HttpContext.TraceIdentifier));
        }

        var command = new ReserveStockCommand(
            request.ProductId,
            request.Quantity,
            request.ReservationId,
            User.Identity?.Name);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? NoContent()
            : result.Error.Type switch
            {
                ErrorType.NotFound => NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                ErrorType.Conflict => Conflict(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                ErrorType.Validation => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                _ => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier))
            };
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