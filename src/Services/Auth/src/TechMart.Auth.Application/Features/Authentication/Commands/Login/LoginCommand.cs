using TechMart.Auth.Application.Common.DTOs;
using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Login;

public record LoginCommand(
    string EmailOrUsername,
    string Password,
    bool RememberMe = false,
    string? IpAddress = null,
    string? UserAgent = null
) : IRequest<Result<LoginResponse>>;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}
