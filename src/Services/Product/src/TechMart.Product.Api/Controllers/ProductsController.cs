using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMart.Product.Application.Common.DTOs;
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
using TechMart.Product.Domain.Product.Enums;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a paginated list of products with optional filtering
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="status">Filter by product status</param>
    /// <param name="categoryId">Filter by category ID</param>
    /// <param name="brandId">Filter by brand ID</param>
    /// <param name="isFeatured">Filter featured products</param>
    /// <param name="isOnSale">Filter products on sale</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDescending">Sort direction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponseDto<ProductDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<PaginatedResponseDto<ProductDto>>>> GetProducts(
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

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<PaginatedResponseDto<ProductDto>>.SuccessResponse(result.Value))
            : BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<ProductDto>.SuccessResponse(result.Value))
            : NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="command">Product creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,ProductManager")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetProduct), 
                new { id = result.Value.Id }, 
                ApiResponse<ProductDto>.SuccessResponse(result.Value, "Product created successfully"));
        }

        return result.Error.Type switch
        {
            ErrorType.Conflict => Conflict(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
            ErrorType.Validation => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
            _ => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier))
        };
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="command">Product update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,ProductManager")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse.FailureResponse("ID mismatch", new[] { "URL ID does not match body ID" }, HttpContext.TraceIdentifier));
        }

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<ProductDto>.SuccessResponse(result.Value, "Product updated successfully"))
            : result.Error.Type switch
            {
                ErrorType.NotFound => NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                ErrorType.Validation => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                _ => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier))
            };
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), 204)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> DeleteProduct(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? NoContent()
            : NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Updates product status
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="status">New status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
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
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? NoContent()
            : result.Error.Type switch
            {
                ErrorType.NotFound => NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                _ => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier))
            };
    }

    /// <summary>
    /// Bulk updates multiple products
    /// </summary>
    /// <param name="command">Bulk update command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpPatch("bulk")]
    [Authorize(Roles = "Admin,ProductManager")]
    [ProducesResponseType(typeof(ApiResponse), 204)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse>> BulkUpdateProducts(
        [FromBody] BulkUpdateProductsCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? NoContent()
            : BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Searches products by text
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="categoryId">Filter by category</param>
    /// <param name="brandId">Filter by brand</param>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <param name="includeInactive">Include inactive products</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponseDto<ProductDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<PaginatedResponseDto<ProductDto>>>> SearchProducts(
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

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<PaginatedResponseDto<ProductDto>>.SuccessResponse(result.Value))
            : BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="includeInactive">Include inactive products</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDescending">Sort direction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Products in category</returns>
    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponseDto<ProductDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PaginatedResponseDto<ProductDto>>>> GetProductsByCategory(
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

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<PaginatedResponseDto<ProductDto>>.SuccessResponse(result.Value))
            : NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Gets products by brand
    /// </summary>
    /// <param name="brandId">Brand ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="includeInactive">Include inactive products</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDescending">Sort direction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Products by brand</returns>
    [HttpGet("brand/{brandId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponseDto<ProductDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PaginatedResponseDto<ProductDto>>>> GetProductsByBrand(
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

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<PaginatedResponseDto<ProductDto>>.SuccessResponse(result.Value))
            : NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }
}