using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.Application.Features.Users.Queries.ValidateRefreshToken;

public sealed record ValidateRefreshTokenQuery(string RefreshToken) : IQuery<TokenValidationVm>;
