using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Category;

namespace TechMart.Product.Infrastructure.Repositories.EntityFramework;

public class CategoryRepository : BaseRepository<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetByParentAsync(Guid? parentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.ParentCategoryId == parentId)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetHierarchyAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var categories = new List<Category>();
        var currentCategory = await GetByIdAsync(categoryId, cancellationToken);
        
        while (currentCategory != null)
        {
            categories.Insert(0, currentCategory);
            
            if (currentCategory.ParentCategoryId.HasValue)
            {
                currentCategory = await GetByIdAsync(currentCategory.ParentCategoryId.Value, cancellationToken);
            }
            else
            {
                break;
            }
        }

        return categories;
    }

    public async Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(c => c.ParentCategoryId == categoryId, cancellationToken);
    }
}