using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Users.Queries.ValidateRefreshToken;

public sealed record ValidateRefreshTokenQuery(string RefreshToken) : IQuery<TokenValidationVm>;
