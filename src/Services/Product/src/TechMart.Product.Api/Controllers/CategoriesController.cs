using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMart.Product.Application.Features.Categories.Commands.CreateCategory;
using TechMart.Product.Application.Features.Categories.Commands.UpdateCategory;
using TechMart.Product.Application.Features.Categories.Queries.GetCategories;
using TechMart.Product.Application.Features.Categories.Queries.GetCategory;
using TechMart.Product.Application.Features.Categories.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all categories with optional filtering
    /// </summary>
    /// <param name="parentCategoryId">Filter by parent category ID</param>
    /// <param name="includeInactive">Include inactive categories</param>
    /// <param name="includeProductCount">Include product count</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryVm>>), 200)]
    public async Task<ActionResult<ApiResponse<List<CategoryVm>>>> GetCategories(
        [FromQuery] Guid? parentCategoryId = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] bool includeProductCount = true,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoriesQuery(parentCategoryId, includeInactive, includeProductCount);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<List<CategoryVm>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Gets a category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryVm>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<CategoryVm>>> GetCategory(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoryQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<CategoryVm>.SuccessResponse(result.Value))
            : NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="command">Category creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,CategoryManager")]
    [ProducesResponseType(typeof(ApiResponse<CategoryVm>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<CategoryVm>>> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetCategory), 
                new { id = result.Value.Id }, 
                ApiResponse<CategoryVm>.SuccessResponse(result.Value, "Category created successfully"));
        }

        return BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="command">Category update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,CategoryManager")]
    [ProducesResponseType(typeof(ApiResponse<CategoryVm>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<CategoryVm>>> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse.FailureResponse("ID mismatch", new[] { "URL ID does not match body ID" }, HttpContext.TraceIdentifier));
        }

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Ok(ApiResponse<CategoryVm>.SuccessResponse(result.Value, "Category updated successfully"))
            : result.Error.Type switch
            {
                ErrorType.NotFound => NotFound(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                ErrorType.Validation => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier)),
                _ => BadRequest(ApiResponse.FailureResponse(result.Error, HttpContext.TraceIdentifier))
            };
    }
}