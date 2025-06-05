using System.Text.Json.Serialization;

namespace TechMart.SharedKernel.Common;

/// <summary>
/// Represents a standardized API response.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Gets the collection of errors, if any.
    /// </summary>
    public IEnumerable<string> Errors { get; init; }

    /// <summary>
    /// Gets the timestamp of the response.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the trace identifier for request correlation.
    /// </summary>
    public string? TraceId { get; init; }

    [JsonConstructor]
    public ApiResponse(bool success, string message, IEnumerable<string>? errors = null, string? traceId = null)
    {
        Success = success;
        Message = message;
        Errors = errors ?? Array.Empty<string>();
        TraceId = traceId;
    }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="message">The success message.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>A successful API response.</returns>
    public static ApiResponse SuccessResponse(string message = "Operation completed successfully", string? traceId = null) =>
        new(true, message, null, traceId);

    /// <summary>
    /// Creates a failed response.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>A failed API response.</returns>
    public static ApiResponse FailureResponse(string message, IEnumerable<string>? errors = null, string? traceId = null) =>
        new(false, message, errors, traceId);

    /// <summary>
    /// Creates a failed response from an Error.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>A failed API response.</returns>
    public static ApiResponse FailureResponse(Error error, string? traceId = null) =>
        new(false, error.Message, new[] { error.Code }, traceId);

    /// <summary>
    /// Creates a response from a Result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="successMessage">The success message.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>An API response based on the result.</returns>
    public static ApiResponse FromResult(Result result, string successMessage = "Operation completed successfully", string? traceId = null) =>
        result.IsSuccess
            ? SuccessResponse(successMessage, traceId)
            : FailureResponse(result.Error, traceId);
}

/// <summary>
/// Represents a standardized API response with data.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Gets the response data.
    /// </summary>
    public T? Data { get; init; }

    [JsonConstructor]
    public ApiResponse(bool success, string message, T? data = default, IEnumerable<string>? errors = null, string? traceId = null)
        : base(success, message, errors, traceId)
    {
        Data = data;
    }

    /// <summary>
    /// Creates a successful response with data.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <param name="message">The success message.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>A successful API response with data.</returns>
    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully", string? traceId = null) =>
        new(true, message, data, null, traceId);

    /// <summary>
    /// Creates a failed response.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>A failed API response.</returns>
    public static new ApiResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null, string? traceId = null) =>
        new(false, message, default, errors, traceId);

    /// <summary>
    /// Creates a failed response from an Error.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>A failed API response.</returns>
    public static new ApiResponse<T> FailureResponse(Error error, string? traceId = null) =>
        new(false, error.Message, default, new[] { error.Code }, traceId);

    /// <summary>
    /// Creates a response from a Result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="successMessage">The success message.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>An API response based on the result.</returns>
    public static ApiResponse<T> FromResult(Result<T> result, string successMessage = "Operation completed successfully", string? traceId = null) =>
        result.IsSuccess
            ? SuccessResponse(result.Value, successMessage, traceId)
            : FailureResponse(result.Error, traceId);
}