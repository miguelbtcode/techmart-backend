using AuthMicroservice.Application.Authentication.Commands;
using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.Contracts.Jwt;
using AuthMicroservice.Application.SocialAuth.Commands;
using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Domain.Events;
using AuthMicroservice.Domain.Interfaces;
using BCrypt.Net;
using MediatR;

namespace AuthMicroservice.Application.SocialAuth.Handlers;

public class GitHubLoginHandler : IRequestHandler<GitHubLoginCommand, Result<LoginResponse>>
{
    private readonly ISocialAuthService _socialAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IMediator _mediator;

    public GitHubLoginHandler(
        ISocialAuthService socialAuthService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IMediator mediator
    )
    {
        _socialAuthService = socialAuthService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _mediator = mediator;
    }

    public async Task<Result<LoginResponse>> Handle(
        GitHubLoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var socialUserInfo = await _socialAuthService.GetGitHubUserInfoAsync(request.AccessToken);

        if (socialUserInfo == null)
        {
            return Result<LoginResponse>.Failure("Failed to get user info from GitHub");
        }

        var user = await _userRepository.GetBySocialLoginAsync("GitHub", socialUserInfo.Id);

        if (user == null)
        {
            // Check if user exists by email
            user = await _userRepository.GetByEmailAsync(socialUserInfo.Email);

            if (user == null)
            {
                // Create new user
                user = new User
                {
                    Email = socialUserInfo.Email.ToLowerInvariant(),
                    Username = socialUserInfo.Email.Split((char)64)[0].ToLowerInvariant(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    FirstName = socialUserInfo.Name,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };

                user = await _userRepository.CreateAsync(user);

                await _mediator.Publish(
                    new UserRegisteredEvent(user.Id, user.Email, user.Username, DateTime.UtcNow),
                    cancellationToken
                );
            }

            // Add social login
            var socialLogin = new SocialLogin
            {
                Provider = "GitHub",
                ProviderUserId = socialUserInfo.Id,
                ProviderEmail = socialUserInfo.Email,
                ProviderName = socialUserInfo.Name,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                LastUsedAt = DateTime.UtcNow,
            };

            user.SocialLogins.Add(socialLogin);
            await _userRepository.UpdateAsync(user);
        }
        else
        {
            var socialLogin = user.SocialLogins.First(sl => sl.Provider == "GitHub");
            socialLogin.UpdateLastUsed();
            await _userRepository.UpdateAsync(user);
        }

        return await GenerateLoginResponse(user, request.RememberMe, cancellationToken);
    }

    private async Task<Result<LoginResponse>> GenerateLoginResponse(
        User user,
        bool rememberMe,
        CancellationToken cancellationToken
    )
    {
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtService.GetRefreshTokenExpirationDays(rememberMe)
            ),
            CreatedAt = DateTime.UtcNow,
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        await _mediator.Publish(
            new UserLoggedInEvent(user.Id, user.Email, null, "Social Login", DateTime.UtcNow),
            cancellationToken
        );

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsEmailVerified = user.IsEmailVerified,
            IsTwoFactorEnabled = user.IsTwoFactorEnabled,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
        };

        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _jwtService.GetAccessTokenExpiration(),
            User = userDto,
        };

        return Result<LoginResponse>.Success(response, "Login successful");
    }
}
