using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.BrandAggregate.Entities;

public class Brand : BaseAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Slug { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private Brand() { }

    public Brand(string name, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = Guard.NotNullOrWhiteSpace(name, nameof(name));
        Description = description;
        Slug = GenerateSlug(name);
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

    public void SetLogo(string? logoUrl)
    {
        LogoUrl = logoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetWebsite(string? websiteUrl)
    {
        WebsiteUrl = websiteUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
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