using TechMart.Product.Domain.Enums;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.ProductAggregate.Entities;

/// <summary>
/// Represents a product review within the product aggregate.
/// </summary>
public class ProductReview : BaseEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public string CustomerName { get; private set; }
    public string? CustomerEmail { get; private set; }
    public int Rating { get; private set; }
    public string? Title { get; private set; }
    public string? Comment { get; private set; }
    public ReviewStatus Status { get; private set; }
    public bool IsVerifiedPurchase { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? ModeratorNotes { get; private set; }

    // Private constructor for EF Core
    private ProductReview() { }

    public ProductReview(
        Guid productId,
        string customerName,
        int rating,
        string? title = null,
        string? comment = null,
        string? customerEmail = null,
        bool isVerifiedPurchase = false)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        CustomerName = !string.IsNullOrWhiteSpace(customerName) ? customerName : 
            throw new ArgumentException("Customer name cannot be empty", nameof(customerName));
        CustomerEmail = customerEmail;
        Rating = ValidateRating(rating);
        Title = title;
        Comment = comment;
        Status = ReviewStatus.Pending;
        IsVerifiedPurchase = isVerifiedPurchase;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateReview(int rating, string? title = null, string? comment = null)
    {
        Rating = ValidateRating(rating);
        Title = title;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string? moderatorNotes = null)
    {
        Status = ReviewStatus.Approved;
        ModeratorNotes = moderatorNotes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string? moderatorNotes = null)
    {
        Status = ReviewStatus.Rejected;
        ModeratorNotes = moderatorNotes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSpam(string? moderatorNotes = null)
    {
        Status = ReviewStatus.Spam;
        ModeratorNotes = moderatorNotes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetVerifiedPurchase(bool isVerified)
    {
        IsVerifiedPurchase = isVerified;
        UpdatedAt = DateTime.UtcNow;
    }

    private static int ValidateRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");
        return rating;
    }

    public bool IsPublic => Status == ReviewStatus.Approved;
    public bool CanBeModified => Status == ReviewStatus.Pending;
}