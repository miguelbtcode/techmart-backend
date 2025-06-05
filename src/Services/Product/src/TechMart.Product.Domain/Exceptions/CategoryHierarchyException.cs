namespace TechMart.Product.Domain.Exceptions;

/// <summary>
/// Exception thrown when category hierarchy operations are invalid.
/// </summary>
public class CategoryHierarchyException : ProductDomainException
{
    public Guid? CategoryId { get; }
    public Guid? ParentCategoryId { get; }

    public CategoryHierarchyException(string message) : base("Product.CategoryHierarchy", message)
    {
    }

    public CategoryHierarchyException(string message, Guid categoryId, Guid? parentCategoryId = null) 
        : base("Product.CategoryHierarchy", message)
    {
        CategoryId = categoryId;
        ParentCategoryId = parentCategoryId;

        WithDetail("CategoryId", categoryId);
        if (parentCategoryId.HasValue)
            WithDetail("ParentCategoryId", parentCategoryId.Value);
    }
    
    public static CategoryHierarchyException CircularReference(Guid categoryId, Guid parentCategoryId) =>
        new($"Category {categoryId} cannot be set as child of {parentCategoryId} - circular reference detected", 
            categoryId, parentCategoryId);
    
    public static CategoryHierarchyException MaxDepthExceeded(Guid categoryId, int maxDepth) =>
        new($"Category {categoryId} exceeds maximum hierarchy depth of {maxDepth}", categoryId);
    
    public static CategoryHierarchyException InvalidParent(Guid categoryId, Guid parentCategoryId) =>
        new($"Category {parentCategoryId} cannot be parent of {categoryId}", categoryId, parentCategoryId);
    
    public static CategoryHierarchyException HasChildren(Guid categoryId) =>
        new($"Category {categoryId} cannot be deleted because it has child categories", categoryId);
}