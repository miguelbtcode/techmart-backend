namespace TechMart.Product.Application.Features.Products.Vms;

public class ProductVariantVm
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
}