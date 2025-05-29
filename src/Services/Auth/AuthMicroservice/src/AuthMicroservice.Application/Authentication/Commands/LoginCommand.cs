using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Authentication.Commands;

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
