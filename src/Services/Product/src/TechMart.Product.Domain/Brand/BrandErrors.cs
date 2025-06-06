using TechMart.SharedKernel.Common;

namespace TechMart.Product.Domain.Brand;

public static class BrandErrors
{
    public static Error BrandNotFound(Guid brandId) => 
        Error.NotFound("Brand.NotFound", $"Brand with ID '{brandId}' not found");
    
    public static Error BrandNameExists(string name) => 
        Error.Conflict("Brand.NameExists", $"Brand with name '{name}' already exists");
    
    public static Error InvalidBrandName() => 
        Error.Validation("Brand.InvalidName", "Brand name cannot be empty");
}