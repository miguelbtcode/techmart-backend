using Microsoft.EntityFrameworkCore;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Infrastructure.Data.EntityFramework;

namespace TechMart.Product.Infrastructure.Repositories.EntityFramework;

public class BrandRepository : BaseRepository<Brand, Guid>, IBrandRepository
{
    public BrandRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Brand?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(b => b.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Brand>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeBrandId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(b => b.Name == name);
        
        if (excludeBrandId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBrandId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}