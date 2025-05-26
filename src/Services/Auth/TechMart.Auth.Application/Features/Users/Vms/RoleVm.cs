namespace TechMart.Auth.Application.Features.Users.Vms;

public sealed record RoleVm(
    Guid Id,
    string Name,
    string Description,
    int HierarchyLevel,
    DateTime AssignedAt
);
