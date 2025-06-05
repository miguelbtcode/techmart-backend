using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Domain.Brand;

public interface IBrandRepository : IRepository<Brand, Guid>
{
    Task<Brand?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Brand>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeBrandId = null, CancellationToken cancellationToken = default);
}