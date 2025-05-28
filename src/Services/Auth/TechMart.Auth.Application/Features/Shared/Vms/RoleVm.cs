namespace TechMart.Auth.Application.Features.Shared.Vms;

public sealed record RoleVm(
    Guid Id,
    string Name,
    string Description,
    int HierarchyLevel,
    DateTime AssignedAt
);
