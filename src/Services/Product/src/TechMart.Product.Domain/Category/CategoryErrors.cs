using TechMart.SharedKernel.Common;

namespace TechMart.Product.Domain.Category;

public static class CategoryErrors
{
    public static Error CategoryNotFound(Guid categoryId) => 
        Error.NotFound("Category.NotFound", $"Category with ID '{categoryId}' not found");
    
    public static Error ParentCategoryNotFound(Guid parentId) => 
        Error.NotFound("Category.ParentNotFound", $"Parent category with ID '{parentId}' not found");
    
    public static Error CircularReference(Guid categoryId, Guid parentId) => 
        Error.Validation("Category.CircularReference", $"Category {categoryId} cannot be child of {parentId}");
}