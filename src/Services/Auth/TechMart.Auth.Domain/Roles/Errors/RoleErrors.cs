using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Domain.Roles.Errors;

public static class RoleErrors
{
    // Role validation errors
    public static Error InvalidId() =>
        Error.Validation("Role.InvalidId", "El ID del rol no puede estar vacío");

    public static Error InvalidName() =>
        Error.Validation("Role.InvalidName", "El nombre del rol no puede estar vacío");

    public static Error InvalidDescription() =>
        Error.Validation("Role.InvalidDescription", "La descripción del rol no puede estar vacía");

    public static Error InvalidHierarchyLevel(int level) =>
        Error.Validation(
            "Role.InvalidHierarchyLevel",
            $"El nivel de jerarquía {level} no es válido. Debe estar entre 1 y 4"
        );

    // Role not found errors
    public static Error RoleNotFound(Guid roleId) =>
        Error.NotFound("Role.NotFound", $"El rol con ID '{roleId}' no fue encontrado");

    public static Error RoleNotFound(string roleName) =>
        Error.NotFound("Role.NotFoundByName", $"El rol '{roleName}' no fue encontrado");

    // User-Role assignment errors
    public static Error InvalidUserId() =>
        Error.Validation("UserRole.InvalidUserId", "El ID del usuario no puede estar vacío");

    public static Error InvalidRoleId() =>
        Error.Validation("UserRole.InvalidRoleId", "El ID del rol no puede estar vacío");

    public static Error UserRoleAlreadyExists(Guid userId, string roleName) =>
        Error.Conflict(
            "UserRole.AlreadyExists",
            $"El usuario ya tiene asignado el rol '{roleName}'"
        );

    public static Error UserRoleNotFound(Guid userId, string roleName) =>
        Error.NotFound("UserRole.NotFound", $"El usuario no tiene asignado el rol '{roleName}'");

    // Business rule errors
    public static Error CannotRemoveLastRole() =>
        Error.Validation(
            "UserRole.CannotRemoveLastRole",
            "No se puede remover el último rol del usuario. Todo usuario debe tener al menos un rol"
        );

    public static Error InsufficientPermissions(string currentRole, string targetRole) =>
        Error.Forbidden(
            "Role.InsufficientPermissions",
            $"El rol '{currentRole}' no tiene permisos suficientes para asignar el rol '{targetRole}'"
        );

    public static Error CannotModifySystemRoles() =>
        Error.Forbidden(
            "Role.CannotModifySystemRoles",
            "Los roles del sistema no pueden ser modificados"
        );

    public static Error CannotAssignMultipleExclusiveRoles() =>
        Error.Validation(
            "UserRole.CannotAssignMultipleExclusiveRoles",
            "No se pueden asignar múltiples roles exclusivos al mismo usuario"
        );
}
