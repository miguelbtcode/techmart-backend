using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Authentication.Queries.ValidateRefreshToken;

public sealed record ValidateRefreshTokenQuery(string RefreshToken)
    : IQuery<ValidateRefreshTokenQueryVm>;
