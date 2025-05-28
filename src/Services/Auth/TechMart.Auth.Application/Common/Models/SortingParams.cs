namespace TechMart.Auth.Application.Common.Models;

/// <summary>
/// Parameters for sorting requests
/// </summary>
public sealed class SortingParams
{
    /// <summary>
    /// Field to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (true for ascending, false for descending)
    /// </summary>
    public bool SortAscending { get; set; } = true;

    /// <summary>
    /// Whether sorting is specified
    /// </summary>
    public bool HasSorting => !string.IsNullOrWhiteSpace(SortBy);

    /// <summary>
    /// Sort direction as string
    /// </summary>
    public string SortDirection => SortAscending ? "asc" : "desc";

    /// <summary>
    /// Creates sorting parameters
    /// </summary>
    public static SortingParams Create(string sortBy, bool ascending = true)
    {
        return new SortingParams { SortBy = sortBy, SortAscending = ascending };
    }

    /// <summary>
    /// Creates ascending sort
    /// </summary>
    public static SortingParams Ascending(string sortBy)
    {
        return Create(sortBy, true);
    }

    /// <summary>
    /// Creates descending sort
    /// </summary>
    public static SortingParams Descending(string sortBy)
    {
        return Create(sortBy, false);
    }

    /// <summary>
    /// Creates default sorting (no sorting)
    /// </summary>
    public static SortingParams None() => new();
}
