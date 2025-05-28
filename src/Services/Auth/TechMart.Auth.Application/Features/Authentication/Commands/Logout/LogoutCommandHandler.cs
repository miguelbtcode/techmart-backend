using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Logout;

internal sealed class LogoutCommandHandler(IRefreshTokenService refreshTokenService)
    : ICommandHandler<LogoutCommand>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await refreshTokenService.RevokeRefreshTokenAsync(request.UserId, cancellationToken);
        return Result.Success();
    }
}
