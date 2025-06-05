using TechMart.Auth.Application.Common.DTOs;
using TechMart.Auth.Application.Common.Results;
using TechMart.Auth.Application.Contracts.Jwt;
using TechMart.Auth.Domain.Entities;
using TechMart.Auth.Domain.Events;
using TechMart.Auth.Domain.Interfaces;
using MediatR;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IMediator _mediator;

    public LoginHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IMediator mediator
    )
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _mediator = mediator;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        // Find user
        User? user = null;
        if (request.EmailOrUsername.Contains("@"))
        {
            user = await _userRepository.GetByEmailAsync(request.EmailOrUsername);
        }
        else
        {
            user = await _userRepository.GetByUsernameAsync(request.EmailOrUsername);
        }

        if (user == null)
        {
            return Result<LoginResponse>.Failure("Invalid credentials");
        }

        if (!user.CanLogin())
        {
            return Result<LoginResponse>.Failure("Account is not active");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure("Invalid credentials");
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new TechMart.Auth.Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtService.GetRefreshTokenExpirationDays(request.RememberMe)
            ),
            CreatedAt = DateTime.UtcNow,
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        // Update user last login
        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        // Publish event
        await _mediator.Publish(
            new UserLoggedInEvent(
                user.Id,
                user.Email,
                request.IpAddress,
                request.UserAgent,
                DateTime.UtcNow
            ),
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
