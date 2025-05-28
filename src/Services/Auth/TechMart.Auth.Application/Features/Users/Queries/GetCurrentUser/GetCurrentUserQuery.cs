using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(string AccessToken) : IQuery<UserDetailVm>;
