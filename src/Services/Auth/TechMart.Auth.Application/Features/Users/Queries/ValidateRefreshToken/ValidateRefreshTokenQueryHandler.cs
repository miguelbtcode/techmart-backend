using TechMart.Auth.Application.Abstractions.Contracts;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Users.Queries.ValidateRefreshToken;

internal sealed class ValidateRefreshTokenQueryHandler(IRefreshTokenService refreshTokenService)
    : IQueryHandler<ValidateRefreshTokenQuery, TokenValidationVm>
{
    public async Task<Result<TokenValidationVm>> Handle(
        ValidateRefreshTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        var validation = await refreshTokenService.ValidateRefreshTokenAsync(
            request.RefreshToken,
            cancellationToken
        );

        var dto = new TokenValidationVm(
            validation.IsValid,
            validation.UserId,
            validation.ExpiresAt
        );

        return Result.Success(dto);
    }
}
