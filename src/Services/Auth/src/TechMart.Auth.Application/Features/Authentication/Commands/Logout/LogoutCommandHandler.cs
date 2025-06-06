using TechMart.Auth.Application.Common.Results;
using TechMart.Auth.Domain.Interfaces;
using MediatR;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var success = await _refreshTokenRepository.RevokeTokenAsync(request.RefreshToken);

        if (success)
        {
            return Result.Success("Logged out successfully");
        }

        return Result.Failure("Failed to logout");
    }
}
