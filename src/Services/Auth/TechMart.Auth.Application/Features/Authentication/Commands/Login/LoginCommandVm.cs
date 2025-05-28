using TechMart.Auth.Application.Features.Shared.Dtos;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Login;

public sealed record LoginCommandVm(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfoVm User
);
