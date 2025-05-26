using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Constants;
using TechMart.Auth.Domain.Roles.Errors;
using TechMart.Auth.Domain.Roles.ValueObjects;

namespace TechMart.Auth.Domain.Roles.Entities;

public sealed class Role : Entity<RoleId>
{
    private readonly List<UserRole> _userRoles = [];

    // EF Core constructor
    private Role()
        : base() { }

    private Role(RoleId id, string name, string description, int hierarchyLevel)
        : base(id)
    {
        Name = name;
        Description = description;
        HierarchyLevel = hierarchyLevel;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int HierarchyLevel { get; private set; }

    // Navigation properties
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    // Factory methods para seeding (solo usado en inicialización del sistema)
    public static Result<Role> CreateAdministrator()
    {
        return new Role(
            RoleId.Administrator,
            RoleConstants.Administrator,
            "Full system access with all permissions",
            RoleConstants.AdministratorLevel
        );
    }

    public static Result<Role> CreateCustomer()
    {
        return new Role(
            RoleId.Customer,
            RoleConstants.Customer,
            "Standard customer with shopping permissions",
            RoleConstants.CustomerLevel
        );
    }

    public static Result<Role> CreateModerator()
    {
        return new Role(
            RoleId.Moderator,
            RoleConstants.Moderator,
            "Content moderation and user management permissions",
            RoleConstants.ModeratorLevel
        );
    }

    public static Result<Role> CreateSupport()
    {
        return new Role(
            RoleId.Support,
            RoleConstants.Support,
            "Customer support and order management permissions",
            RoleConstants.SupportLevel
        );
    }

    // Factory method genérico para tests o casos especiales
    internal static Result<Role> Create(
        RoleId id,
        string name,
        string description,
        int hierarchyLevel
    )
    {
        if (id == Guid.Empty)
            return RoleErrors.InvalidId();

        if (string.IsNullOrWhiteSpace(name))
            return RoleErrors.InvalidName();

        if (string.IsNullOrWhiteSpace(description))
            return RoleErrors.InvalidDescription();

        if (hierarchyLevel < 1 || hierarchyLevel > 4)
            return RoleErrors.InvalidHierarchyLevel(hierarchyLevel);

        return new Role(id, name, description, hierarchyLevel);
    }

    // Domain methods
    public bool CanAssignRole(Role targetRole)
    {
        // Solo roles con mayor jerarquía pueden asignar otros roles
        return HierarchyLevel > targetRole.HierarchyLevel;
    }

    public bool IsHigherThan(Role otherRole)
    {
        return HierarchyLevel > otherRole.HierarchyLevel;
    }

    public bool IsAdministrator() => Name == RoleConstants.Administrator;

    public bool IsCustomer() => Name == RoleConstants.Customer;

    public bool IsModerator() => Name == RoleConstants.Moderator;

    public bool IsSupport() => Name == RoleConstants.Support;
}
