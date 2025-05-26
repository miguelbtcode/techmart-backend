using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Domain.Users.Repository;

/// <summary>
/// Repository interface for User aggregate
/// </summary>
public interface IUserRepository : IRepository<User, UserId>
{
    /// <summary>
    /// Gets a user by their email address (para login)
    /// </summary>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if email is unique (para registro)
    /// </summary>
    Task<bool> IsEmailUniqueAsync(
        Email email,
        UserId? excludeUserId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets user by email with roles loaded (para autorización)
    /// </summary>
    Task<User?> GetByEmailWithRolesAsync(
        Email email,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets user by ID with roles loaded (para autorización en requests autenticados)
    /// </summary>
    Task<User?> GetByIdWithRolesAsync(UserId id, CancellationToken cancellationToken = default);
}
