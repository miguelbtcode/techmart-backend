using System.Text.Json.Serialization;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.API.Common.Responses;

/// <summary>
/// Respuesta estandarizada para todas las APIs (versión genérica)
/// </summary>
/// <typeparam name="T">Tipo de datos que retorna la respuesta</typeparam>
public sealed class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; private set; }

    [JsonPropertyName("message")]
    public string Message { get; private set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; private set; }

    [JsonPropertyName("errors")]
    public IReadOnlyList<ApiError> Errors { get; private set; } = [];

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    [JsonPropertyName("traceId")]
    public string TraceId { get; private set; } = string.Empty;

    private ApiResponse() { }

    // ✅ Respuestas de Éxito
    public static ApiResponse<T> Success(
        T data,
        string message = "Operation completed successfully"
    ) =>
        new()
        {
            IsSuccess = true,
            Message = message,
            Data = data,
        };

    // ✅ Respuestas de Error
    public static ApiResponse<T> Failure(string message, params ApiError[] errors) =>
        new()
        {
            IsSuccess = false,
            Message = message,
            Errors = errors?.ToList().AsReadOnly() ?? new List<ApiError>().AsReadOnly(),
        };

    public static ApiResponse<T> Failure(string message) =>
        new() { IsSuccess = false, Message = message };

    // ✅ Desde Result Pattern
    public static ApiResponse<T> FromResult(Result<T> result, string? successMessage = null)
    {
        if (result.IsSuccess)
            return Success(result.Value, successMessage ?? "Operation completed successfully");

        return Failure(
            "Operation failed",
            new ApiError(result.Error.Code, result.Error.Message, result.Error.Type.ToString())
        );
    }

    // ✅ Desde ValidationError
    public static ApiResponse<T> FromValidationError(ValidationError validationError)
    {
        var errors = validationError
            .Errors.Select(e => new ApiError(e.Code, e.Message, "Validation"))
            .ToArray();

        return Failure("Validation failed", errors);
    }

    // ✅ Para establecer TraceId
    public ApiResponse<T> WithTraceId(string traceId)
    {
        TraceId = traceId;
        return this;
    }
}

/// <summary>
/// Respuesta estandarizada sin datos (non-generic para casos simples)
/// </summary>
public sealed class ApiResponse
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; private set; }

    [JsonPropertyName("message")]
    public string Message { get; private set; } = string.Empty;

    [JsonPropertyName("errors")]
    public IReadOnlyList<ApiError> Errors { get; private set; } = [];

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    [JsonPropertyName("traceId")]
    public string TraceId { get; private set; } = string.Empty;

    private ApiResponse() { }

    // ✅ Respuestas de Éxito (sin datos)
    public static ApiResponse Success(string message = "Operation completed successfully") =>
        new() { IsSuccess = true, Message = message };

    // ✅ Respuestas de Éxito (con datos - crea la versión genérica)
    public static ApiResponse<T> Success<T>(
        T data,
        string message = "Operation completed successfully"
    ) => ApiResponse<T>.Success(data, message);

    // ✅ Respuestas de Error
    public static ApiResponse Failure(string message, params ApiError[] errors) =>
        new()
        {
            IsSuccess = false,
            Message = message,
            Errors = errors?.ToList().AsReadOnly() ?? new List<ApiError>().AsReadOnly(),
        };

    public static ApiResponse Failure(string message) =>
        new() { IsSuccess = false, Message = message };

    // ✅ Desde Result Pattern (sin datos)
    public static ApiResponse FromResult(Result result, string? successMessage = null)
    {
        if (result.IsSuccess)
            return Success(successMessage ?? "Operation completed successfully");

        return Failure(
            "Operation failed",
            new ApiError(result.Error.Code, result.Error.Message, result.Error.Type.ToString())
        );
    }

    // ✅ Desde Result Pattern (con datos - crea la versión genérica)
    public static ApiResponse<T> FromResult<T>(Result<T> result, string? successMessage = null) =>
        ApiResponse<T>.FromResult(result, successMessage);

    // ✅ Desde ValidationError
    public static ApiResponse FromValidationError(ValidationError validationError)
    {
        var errors = validationError
            .Errors.Select(e => new ApiError(e.Code, e.Message, "Validation"))
            .ToArray();

        return Failure("Validation failed", errors);
    }

    // ✅ Para establecer TraceId
    public ApiResponse WithTraceId(string traceId)
    {
        TraceId = traceId;
        return this;
    }
}
