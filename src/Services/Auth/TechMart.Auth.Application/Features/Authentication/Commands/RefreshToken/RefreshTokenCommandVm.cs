namespace TechMart.Auth.Application.Features.Authentication.Commands.RefreshToken;

public sealed record RefreshTokenCommandVm(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
