using AuthMicroservice.Models.DTOs;

namespace AuthMicroservice.Services;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> LogoutAsync(string refreshToken);
}
