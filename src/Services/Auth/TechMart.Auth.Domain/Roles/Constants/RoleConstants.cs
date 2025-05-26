using TechMart.Auth.Domain.Roles.ValueObjects;

namespace TechMart.Auth.Domain.Roles.Constants;

public static class RoleConstants
{
    // Role Names
    public const string Administrator = "Administrator";
    public const string Customer = "Customer";
    public const string Moderator = "Moderator";
    public const string Support = "Support";

    // Role IDs (deterministic GUIDs basados en el nombre)
    public static readonly RoleId AdministratorId = RoleId.From(
        new Guid("00000000-0000-0000-0000-000000000001")
    );
    public static readonly RoleId CustomerId = RoleId.From(
        new Guid("00000000-0000-0000-0000-000000000002")
    );
    public static readonly RoleId ModeratorId = RoleId.From(
        new Guid("00000000-0000-0000-0000-000000000003")
    );
    public static readonly RoleId SupportId = RoleId.From(
        new Guid("00000000-0000-0000-0000-000000000004")
    );

    // Default Role
    public static readonly Guid DefaultRoleId = CustomerId;
    public const string DefaultRoleName = Customer;

    // Role Hierarchy Levels (higher number = more permissions)
    public const int CustomerLevel = 1;
    public const int SupportLevel = 2;
    public const int ModeratorLevel = 3;
    public const int AdministratorLevel = 4;

    // Collections
    public static readonly IReadOnlyList<string> AllRoleNames = new[]
    {
        Administrator,
        Customer,
        Moderator,
        Support,
    };

    public static readonly IReadOnlyDictionary<Guid, string> RoleIdToName = new Dictionary<
        Guid,
        string
    >
    {
        { AdministratorId, Administrator },
        { CustomerId, Customer },
        { ModeratorId, Moderator },
        { SupportId, Support },
    };

    public static readonly IReadOnlyDictionary<string, Guid> RoleNameToId = new Dictionary<
        string,
        Guid
    >
    {
        { Administrator, AdministratorId },
        { Customer, CustomerId },
        { Moderator, ModeratorId },
        { Support, SupportId },
    };

    public static readonly IReadOnlyDictionary<string, int> RoleHierarchy = new Dictionary<
        string,
        int
    >
    {
        { Customer, CustomerLevel },
        { Support, SupportLevel },
        { Moderator, ModeratorLevel },
        { Administrator, AdministratorLevel },
    };
}
