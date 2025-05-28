using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Authentication.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(string AccessToken) : IQuery<GetCurrentUserQueryVm>;
