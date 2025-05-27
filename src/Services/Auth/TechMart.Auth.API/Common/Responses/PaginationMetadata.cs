using System.Text.Json.Serialization;

namespace TechMart.Auth.API.Common.Responses;

/// <summary>
/// Metadatos de paginación
/// </summary>
public sealed record PaginationMetadata
{
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }

    [JsonPropertyName("pageIndex")]
    public int PageIndex { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage => PageIndex < TotalPages;

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage => PageIndex > 1;

    public PaginationMetadata(int totalCount, int pageIndex, int pageSize)
    {
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}
