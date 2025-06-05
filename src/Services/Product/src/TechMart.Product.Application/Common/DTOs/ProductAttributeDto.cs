namespace TechMart.Product.Application.Common.DTOs;

public class ProductAttributeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Unit { get; set; }
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }
}