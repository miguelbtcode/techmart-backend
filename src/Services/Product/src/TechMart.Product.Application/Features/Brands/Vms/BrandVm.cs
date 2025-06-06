namespace TechMart.Product.Application.Features.Brands.Vms;

public record BrandVm(
    Guid Id, 
    string Name, 
    string? Description, 
    string Slug, 
    string? LogoUrl,
    string? WebsiteUrl,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt,
    int ProductCount);