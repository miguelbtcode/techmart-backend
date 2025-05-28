using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Roles.Queries.GetRoles;

public sealed record GetRolesQuery : IQuery<IEnumerable<GetRolesQueryVm>>;
