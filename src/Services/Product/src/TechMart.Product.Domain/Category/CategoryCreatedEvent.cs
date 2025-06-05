using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Category;

public class CategoryCreatedEvent : BaseDomainEvent
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public Guid? ParentCategoryId { get; }

    public CategoryCreatedEvent(Guid categoryId, string name, Guid? parentCategoryId)
    {
        CategoryId = categoryId;
        Name = name;
        ParentCategoryId = parentCategoryId;
    }
}