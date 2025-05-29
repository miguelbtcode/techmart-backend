using AuthMicroservice.Application.Authentication.Commands;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Domain.Interfaces;
using MediatR;

namespace AuthMicroservice.Application.Authentication.Handlers;

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
