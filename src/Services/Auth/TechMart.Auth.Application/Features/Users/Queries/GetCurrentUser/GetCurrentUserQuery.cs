using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(string AccessToken) : IQuery<UserDetailVm>;
