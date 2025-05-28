namespace TechMart.Auth.Application.Features.Roles.Queries.GetRoles;

public sealed record GetRolesQueryVm(
    Guid Id,
    string Name,
    string Description,
    int HierarchyLevel,
    DateTime CreatedAt,
    int UserCount = 0
);
