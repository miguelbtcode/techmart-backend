using Microsoft.AspNetCore.Mvc;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Maneja Result<T> y devuelve ActionResult apropiado
    /// </summary>
    protected ActionResult<ApiResponse<T>> HandleResult<T>(Result<T> result, string? successMessage = null)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<T>.SuccessResponse(result.Value, successMessage ?? "Operation completed successfully"));

        return MapErrorToActionResult<T>(result.Error);
    }

    /// <summary>
    /// Maneja Result sin valor y devuelve ActionResult apropiado
    /// </summary>
    protected ActionResult<ApiResponse> HandleResult(Result result, string? successMessage = null)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse.SuccessResponse(successMessage ?? "Operation completed successfully"));

        return MapErrorToActionResult(result.Error);
    }

    /// <summary>
    /// Maneja Result<T> para operaciones CREATE (201 Created)
    /// </summary>
    protected ActionResult<ApiResponse<T>> HandleCreatedResult<T>(
        Result<T> result, 
        string actionName, 
        object routeValues, 
        string? successMessage = null)
    {
        if (result.IsSuccess)
            return CreatedAtAction(actionName, routeValues, 
                ApiResponse<T>.SuccessResponse(result.Value, successMessage ?? "Resource created successfully"));

        return MapErrorToActionResult<T>(result.Error);
    }

    /// <summary>
    /// Maneja Result sin valor para operaciones DELETE (204 No Content)
    /// </summary>
    protected ActionResult<ApiResponse> HandleDeletedResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        return MapErrorToActionResult(result.Error);
    }
    
    /// <summary>
    /// Maneja Result sin valor para operaciones UPDATE (204 No Content)
    /// </summary>
    protected ActionResult<ApiResponse> HandleUpdatedResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        return MapErrorToActionResult(result.Error);
    }

    private ActionResult<ApiResponse<T>> MapErrorToActionResult<T>(Error error)
    {
        var apiResponse = ApiResponse<T>.FailureResponse(error, HttpContext.TraceIdentifier);
        
        return error.Type switch
        {
            ErrorType.NotFound => NotFound(apiResponse),
            ErrorType.Conflict => Conflict(apiResponse),
            ErrorType.Validation => BadRequest(apiResponse),
            ErrorType.Unauthorized => Unauthorized(apiResponse),
            ErrorType.Forbidden => StatusCode(403, apiResponse),
            _ => StatusCode(500, apiResponse)
        };
    }

    private ActionResult<ApiResponse> MapErrorToActionResult(Error error)
    {
        var apiResponse = ApiResponse.FailureResponse(
            error,
            HttpContext.TraceIdentifier);
        
        return error.Type switch
        {
            ErrorType.NotFound => NotFound(apiResponse),
            ErrorType.Conflict => Conflict(apiResponse),
            ErrorType.Validation => BadRequest(apiResponse),
            ErrorType.Unauthorized => Unauthorized(apiResponse),
            ErrorType.Forbidden => StatusCode(403, apiResponse),
            _ => StatusCode(500, apiResponse)
        };
    }
}