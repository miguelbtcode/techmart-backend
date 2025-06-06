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

[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController(IMediator mediator) : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryVm>>), 200)]
    public async Task<ActionResult<ApiResponse<List<CategoryVm>>>> GetCategories(
        [FromQuery] Guid? parentCategoryId = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] bool includeProductCount = true,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoriesQuery(parentCategoryId, includeInactive, includeProductCount);
        var result = await mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryVm>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<CategoryVm>>> GetCategory(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoryQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CategoryManager")]
    [ProducesResponseType(typeof(ApiResponse<CategoryVm>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<CategoryVm>>> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        return HandleCreatedResult(result, nameof(GetCategory), new { id = result.Value?.Id });
    }

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
            return BadRequest(ApiResponse.FailureResponse(
                "ID mismatch",
                ["URL ID does not match body ID"], 
                HttpContext.TraceIdentifier));
        }

        var result = await mediator.Send(command, cancellationToken);

        return HandleResult(result, "Category updated successfully");
    }
}