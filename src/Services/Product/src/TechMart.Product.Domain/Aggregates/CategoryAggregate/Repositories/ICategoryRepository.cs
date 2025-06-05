using TechMart.Product.Domain.Aggregates.CategoryAggregate.Entities;
using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Domain.Aggregates.CategoryAggregate.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetByParentAsync(Guid? parentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetHierarchyAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default);
}