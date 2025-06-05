using TechMart.Product.Domain.Enums;

namespace TechMart.Product.Application.Common.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public CategoryStatus Status { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CategoryDto> Children { get; set; } = new();
    public int ProductCount { get; set; }
}