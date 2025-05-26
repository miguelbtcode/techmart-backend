using TechMart.Auth.Application.Abstractions.Authentication;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Users.Queries.ValidateRefreshToken;

internal sealed class ValidateRefreshTokenQueryHandler(IJwtProvider jwtProvider)
    : IQueryHandler<ValidateRefreshTokenQuery, TokenValidationVm>
{
    public async Task<Result<TokenValidationVm>> Handle(
        ValidateRefreshTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        var validation = await jwtProvider.ValidateRefreshTokenAsync(
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
