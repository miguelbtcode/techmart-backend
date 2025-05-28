using TechMart.Auth.Application.Features.Shared.Vms;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserRoles;

public sealed record GetUserRolesQueryVm(Guid UserId, IEnumerable<RoleVm> Roles);
