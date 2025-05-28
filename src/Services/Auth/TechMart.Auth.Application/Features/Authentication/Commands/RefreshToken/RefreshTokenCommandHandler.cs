using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Authentication.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IRefreshTokenService refreshTokenService,
    IUnitOfWork unitOfWork,
    IJwtProvider jwtProvider
) : ICommandHandler<RefreshTokenCommand, RefreshTokenCommandVm>
{
    public async Task<Result<RefreshTokenCommandVm>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate refresh token
        var validation = await refreshTokenService.ValidateRefreshTokenAsync(
            request.RefreshToken,
            cancellationToken
        );

        if (!validation.IsValid || validation.UserId == null)
            return Result.Failure<RefreshTokenCommandVm>(
                Error.Unauthorized(
                    "RefreshToken.Invalid",
                    "The refresh token is invalid or expired"
                )
            );

        // Get user
        var userId = UserId.From(validation.UserId.Value);
        var user = await unitOfWork.Users.GetByIdWithRolesAsync(userId, cancellationToken);

        if (user == null)
            return Result.Failure<RefreshTokenCommandVm>(
                UserErrors.NotFound(validation.UserId.Value)
            );

        // Check if user can still login
        if (!user.CanLogin())
            return Result.Failure<RefreshTokenCommandVm>(UserErrors.CannotLogin(user.Status));

        // Generate new tokens
        var newAccessToken = jwtProvider.GenerateToken(user);
        var newRefreshToken = await refreshTokenService.GenerateRefreshTokenAsync(
            user,
            cancellationToken
        );

        // Revoke old refresh token
        await refreshTokenService.RevokeRefreshTokenAsync(user.Id.Value, cancellationToken);

        var expiresAt = DateTime.UtcNow.AddHours(1); // Get from JWT settings

        return Result.Success(
            new RefreshTokenCommandVm(newAccessToken, newRefreshToken, expiresAt)
        );
    }
}
