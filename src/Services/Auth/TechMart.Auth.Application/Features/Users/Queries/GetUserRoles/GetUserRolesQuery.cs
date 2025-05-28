using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserRoles;

public sealed record GetUserRolesQuery(Guid UserId) : IQuery<UserRolesVm>;
