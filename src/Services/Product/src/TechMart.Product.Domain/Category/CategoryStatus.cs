namespace TechMart.Product.Domain.Category;

/// <summary>
/// Represents the status of a category.
/// </summary>
public enum CategoryStatus
{
    /// <summary>
    /// Category is active and visible.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Category is inactive but not deleted.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Category is archived.
    /// </summary>
    Archived = 3
}