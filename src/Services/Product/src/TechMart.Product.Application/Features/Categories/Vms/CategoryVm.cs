using TechMart.Product.Domain.Category;

namespace TechMart.Product.Application.Features.Categories.Vms;

public record CategoryVm
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Slug { get; init; } = string.Empty;
    public Guid? ParentCategoryId { get; init; }
    public string? ParentCategoryName { get; init; }
    public CategoryStatus Status { get; init; }
    public bool IsActive { get; init; }
    public int SortOrder { get; init; }
    public string? ImageUrl { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<CategoryVm> Children { get; init; } = [];
    public int ProductCount { get; set; }
}