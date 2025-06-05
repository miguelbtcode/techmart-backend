using TechMart.SharedKernel.Base;
using TechMart.Product.Domain.Enums;

namespace TechMart.Product.Domain.Aggregates.CategoryAggregate.Entities;

public class Category : BaseAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Slug { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public CategoryStatus Status { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public virtual Category? ParentCategory { get; private set; }
    public virtual ICollection<Category> ChildCategories { get; private set; } = new List<Category>();

    // Private constructor for EF Core
    private Category() { }

    public Category(string name, string? description = null, Guid? parentCategoryId = null)
    {
        Id = Guid.NewGuid();
        Name = Guard.NotNullOrWhiteSpace(name, nameof(name));
        Description = description;
        Slug = GenerateSlug(name);
        ParentCategoryId = parentCategoryId;
        Status = CategoryStatus.Active;
        IsActive = true;
        SortOrder = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string name, string? description = null)
    {
        Name = Guard.NotNullOrWhiteSpace(name, nameof(name));
        Description = description;
        Slug = GenerateSlug(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetParent(Guid? parentCategoryId)
    {
        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = CategoryStatus.Active;
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = CategoryStatus.Inactive;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
                  .Replace(" ", "-")
                  .Replace("&", "and");
    }

    private static class Guard
    {
        public static string NotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
            return value;
        }
    }
}