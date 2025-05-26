namespace TechMart.Auth.Application.Features.Users.Vms;

public sealed record UserRolesVm(Guid UserId, IEnumerable<RoleVm> Roles);
