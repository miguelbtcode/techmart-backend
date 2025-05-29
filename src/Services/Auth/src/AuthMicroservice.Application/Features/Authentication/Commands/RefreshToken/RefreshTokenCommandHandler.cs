using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.Contracts.Jwt;
using AuthMicroservice.Application.Features.Authentication.Commands.Login;
using AuthMicroservice.Domain.Interfaces;
using MediatR;

namespace AuthMicroservice.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        // Find refresh token
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (refreshToken == null)
        {
            return Result<LoginResponse>.Failure("Invalid or expired refresh token");
        }

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.User);
        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();

        // Revoke old refresh token
        await _refreshTokenRepository.RevokeTokenAsync(request.RefreshToken);

        // Create new refresh token
        var newRefreshToken = new AuthMicroservice.Domain.Entities.RefreshToken
        {
            Token = newRefreshTokenValue,
            UserId = refreshToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtService.GetRefreshTokenExpirationDays(false)),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
        };

        await _refreshTokenRepository.CreateAsync(newRefreshToken);

        var userDto = new UserDto
        {
            Id = refreshToken.User.Id,
            Email = refreshToken.User.Email,
            Username = refreshToken.User.Username,
            FirstName = refreshToken.User.FirstName,
            LastName = refreshToken.User.LastName,
            IsEmailVerified = refreshToken.User.IsEmailVerified,
            IsTwoFactorEnabled = refreshToken.User.IsTwoFactorEnabled,
            CreatedAt = refreshToken.User.CreatedAt,
            LastLoginAt = refreshToken.User.LastLoginAt,
        };

        var response = new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = _jwtService.GetAccessTokenExpiration(),
            User = userDto,
        };

        return Result<LoginResponse>.Success(response, "Token refreshed successfully");
    }
}
