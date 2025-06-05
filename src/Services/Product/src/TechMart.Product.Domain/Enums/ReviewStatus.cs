using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Enums;

/// <summary>
/// Represents the status of a review.
/// </summary>
public enum ReviewStatus
{
    /// <summary>
    /// Review is pending moderation.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Review has been approved and is visible.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Review has been rejected and is not visible.
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Review has been marked as spam.
    /// </summary>
    Spam = 4
}

/// <summary>
/// Type-safe enumeration for ReviewStatus with additional functionality.
/// </summary>
public class ReviewStatusEnumeration : Enumeration
{
    public static readonly ReviewStatusEnumeration Pending = new(1, nameof(Pending), "Pending Review", false);
    public static readonly ReviewStatusEnumeration Approved = new(2, nameof(Approved), "Approved", true);
    public static readonly ReviewStatusEnumeration Rejected = new(3, nameof(Rejected), "Rejected", false);
    public static readonly ReviewStatusEnumeration Spam = new(4, nameof(Spam), "Spam", false);

    public string DisplayName { get; }
    public bool IsVisible { get; }

    private ReviewStatusEnumeration(int id, string name, string displayName, bool isVisible) : base(id, name)
    {
        DisplayName = displayName;
        IsVisible = isVisible;
    }

    /// <summary>
    /// Gets a value indicating whether the review can be displayed to customers.
    /// </summary>
    public bool CanBeDisplayed => IsVisible;

    /// <summary>
    /// Gets a value indicating whether the review requires moderation.
    /// </summary>
    public bool RequiresModeration => this == Pending;

    /// <summary>
    /// Gets all review statuses that are visible to customers.
    /// </summary>
    public static IEnumerable<ReviewStatusEnumeration> GetVisibleStatuses() =>
        new[] { Approved };

    /// <summary>
    /// Gets all review statuses that require moderation action.
    /// </summary>
    public static IEnumerable<ReviewStatusEnumeration> GetModerationStatuses() =>
        new[] { Pending };
}