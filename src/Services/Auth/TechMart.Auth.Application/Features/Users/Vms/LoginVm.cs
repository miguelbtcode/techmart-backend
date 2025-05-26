namespace TechMart.Auth.Application.Features.Users.Vms;

public sealed record LoginVm(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfoVm User
);
