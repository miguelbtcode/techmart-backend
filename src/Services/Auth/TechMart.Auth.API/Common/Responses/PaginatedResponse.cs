using System.Text.Json.Serialization;

namespace TechMart.Auth.API.Common.Responses;

/// <summary>
/// Respuesta paginada estandarizada
/// </summary>
public sealed class PaginatedResponse<T>
    where T : class
{
    [JsonPropertyName("IsSuccess")]
    public bool IsSuccess { get; private set; }

    [JsonPropertyName("message")]
    public string Message { get; private set; } = string.Empty;

    [JsonPropertyName("data")]
    public IReadOnlyList<T> Data { get; private set; } = Array.Empty<T>();

    [JsonPropertyName("pagination")]
    public PaginationMetadata Pagination { get; private set; } = null!;

    [JsonPropertyName("errors")]
    public IReadOnlyList<ApiError> Errors { get; private set; } = Array.Empty<ApiError>();

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    [JsonPropertyName("traceId")]
    public string TraceId { get; private set; } = string.Empty;

    private PaginatedResponse() { }

    public static PaginatedResponse<T> Success(
        IReadOnlyList<T> data,
        int totalCount,
        int pageIndex,
        int pageSize,
        string message = "Data retrieved successfully"
    )
    {
        return new()
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Pagination = new PaginationMetadata(totalCount, pageIndex, pageSize),
        };
    }

    public static PaginatedResponse<T> Failure(string message, params ApiError[] errors)
    {
        return new()
        {
            IsSuccess = false,
            Message = message,
            Errors = errors?.ToList().AsReadOnly() ?? new List<ApiError>().AsReadOnly(),
            Data = [],
            Pagination = new PaginationMetadata(0, 1, 10),
        };
    }

    public PaginatedResponse<T> WithTraceId(string traceId)
    {
        TraceId = traceId;
        return this;
    }
}
