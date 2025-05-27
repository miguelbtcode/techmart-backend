using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.Application.Features.Shared.Vms;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.API.Extensions;

/// <summary>
/// Extensions para convertir Results a HTTP Responses
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Convierte un Result a una respuesta HTTP apropiada
    /// </summary>
    public static IResult ToHttpResult(this Result result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse.Success(
                successMessage ?? "Operation completed successfully"
            );
            return Results.Ok(response);
        }

        return CreateErrorResult(result.Error);
    }

    /// <summary>
    /// Convierte un Result<T> a una respuesta HTTP apropiada
    /// </summary>
    public static IResult ToHttpResult<T>(this Result<T> result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse.Success(
                result.Value,
                successMessage ?? "Operation completed successfully"
            );
            return Results.Ok(response);
        }

        return CreateErrorResult(result.Error);
    }

    /// <summary>
    /// Convierte un Result paginado a respuesta HTTP
    /// </summary>
    public static IResult ToPaginatedHttpResult<T>(
        this Result<PaginationVm<T>> result,
        string? successMessage = null
    )
        where T : class
    {
        if (result.IsSuccess)
        {
            var data = result.Value;
            var response = PaginatedResponse<T>.Success(
                (data.Data ?? Enumerable.Empty<T>()).ToList().AsReadOnly(),
                data.Count,
                data.PageIndex,
                data.PageSize,
                successMessage ?? "Data retrieved successfully"
            );
            return Results.Ok(response);
        }

        return CreateErrorResult(result.Error);
    }

    /// <summary>
    /// Convierte un Result a Created response
    /// </summary>
    public static IResult ToCreatedResult<T>(
        this Result<T> result,
        string location,
        string? successMessage = null
    )
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse.Success(
                result.Value,
                successMessage ?? "Resource created successfully"
            );
            return Results.Created(location, response);
        }

        return CreateErrorResult(result.Error);
    }

    private static IResult CreateErrorResult(Error error)
    {
        var apiError = new ApiError(error.Code, error.Message, error.Type.ToString());
        var response = ApiResponse.Failure("Operation failed", apiError);

        return error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(response),
            ErrorType.NotFound => Results.NotFound(response),
            ErrorType.Conflict => Results.Conflict(response),
            ErrorType.Unauthorized => Results.Json(
                response,
                statusCode: StatusCodes.Status401Unauthorized
            ),
            ErrorType.Forbidden => Results.Json(
                response,
                statusCode: StatusCodes.Status403Forbidden
            ),
            _ => Results.Json(response, statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
