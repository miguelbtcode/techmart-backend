using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Authentication.Queries.ValidateRefreshToken;

internal sealed class ValidateRefreshTokenQueryHandler(IRefreshTokenService refreshTokenService)
    : IQueryHandler<ValidateRefreshTokenQuery, ValidateRefreshTokenQueryVm>
{
    public async Task<Result<ValidateRefreshTokenQueryVm>> Handle(
        ValidateRefreshTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        var validation = await refreshTokenService.ValidateRefreshTokenAsync(
            request.RefreshToken,
            cancellationToken
        );

        var dto = new ValidateRefreshTokenQueryVm(
            validation.IsValid,
            validation.UserId,
            validation.ExpiresAt
        );

        return Result.Success(dto);
    }
}
