using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMart.Product.Application.Features.Products.Commands.BulkUpdateProducts;
using TechMart.Product.Application.Features.Products.Commands.CreateProduct;
using TechMart.Product.Application.Features.Products.Commands.DeleteProduct;
using TechMart.Product.Application.Features.Products.Commands.UpdateProduct;
using TechMart.Product.Application.Features.Products.Commands.UpdateProductStatus;
using TechMart.Product.Application.Features.Products.Queries.GetProduct;
using TechMart.Product.Application.Features.Products.Queries.GetProducts;
using TechMart.Product.Application.Features.Products.Queries.GetProductsByBrand;
using TechMart.Product.Application.Features.Products.Queries.GetProductsByCategory;
using TechMart.Product.Application.Features.Products.Queries.SearchProducts;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Product.Enums;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(IMediator mediator) : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponseVm<ProductVm>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<PaginatedResponseVm<ProductVm>>>> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ProductStatus? status = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? brandId = null,
        [FromQuery] bool? isFeatured = null,
        [FromQuery] bool? isOnSale = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(
            pageNumber, pageSize, status, categoryId, brandId, 
            isFeatured, isOnSale, minPrice, maxPrice, sortBy, sortDescending);

        var result = await mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductVm>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<ProductVm>>> GetProduct(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ProductManager")]
    [ProducesResponseType(typeof(ApiResponse<ProductVm>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<ActionResult<ApiResponse<ProductVm>>> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        return HandleCreatedResult(result, nameof(GetProduct), new { id = result.Value?.Id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,ProductManager")]
    [ProducesResponseType(typeof(ApiResponse<ProductVm>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<ProductVm>>> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse.FailureResponse(
                "ID mismatch", 
                new[] { "URL ID does not match body ID" }, 
                HttpContext.TraceIdentifier));
        }

        var result = await mediator.Send(command, cancellationToken);

        return HandleResult(result, "Product updated successfully");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), 204)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> DeleteProduct(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteProductCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return HandleDeletedResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,ProductManager")]
    [ProducesResponseType(typeof(ApiResponse), 204)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> UpdateProductStatus(
        Guid id,
        [FromBody] ProductStatus status,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateProductStatusCommand(id, status);
        var result = await mediator.Send(command, cancellationToken);

        return HandleDeletedResult(result);
    }

    [HttpPatch("bulk")]
    [Authorize(Roles = "Admin,ProductManager")]
    [ProducesResponseType(typeof(ApiResponse), 204)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse>> BulkUpdateProducts(
        [FromBody] BulkUpdateProductsCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        return HandleDeletedResult(result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponseVm<ProductVm>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<PaginatedResponseVm<ProductVm>>>> SearchProducts(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? brandId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchProductsQuery(
            searchTerm, pageNumber, pageSize, categoryId, brandId, minPrice, maxPrice, includeInactive);

        var result = await mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponseVm<ProductVm>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PaginatedResponseVm<ProductVm>>>> GetProductsByCategory(
        Guid categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsByCategoryQuery(
            categoryId, pageNumber, pageSize, includeInactive, sortBy, sortDescending);

        var result = await mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("brand/{brandId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponseVm<ProductVm>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PaginatedResponseVm<ProductVm>>>> GetProductsByBrand(
        Guid brandId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsByBrandQuery(
            brandId, pageNumber, pageSize, includeInactive, sortBy, sortDescending);

        var result = await mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }
}