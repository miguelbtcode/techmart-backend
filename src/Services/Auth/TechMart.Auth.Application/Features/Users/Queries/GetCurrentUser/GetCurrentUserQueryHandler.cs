using TechMart.Auth.Application.Abstractions.Authentication;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Queries.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler(IUnitOfWork unitOfWork, IJwtProvider jwtProvider)
    : IQueryHandler<GetCurrentUserQuery, UserDetailVm>
{
    public async Task<Result<UserDetailVm>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken
    )
    {
        // Validar y decodificar el token
        var tokenValidation = await jwtProvider.ValidateAccessTokenAsync(
            request.AccessToken,
            cancellationToken
        );

        if (!tokenValidation.IsValid || tokenValidation.UserId is null)
            return Result.Failure<UserDetailVm>(
                Error.Unauthorized("Token.Invalid", "El token es inválido o ha expirado")
            );

        var userId = UserId.From(tokenValidation.UserId.Value);

        var user = await unitOfWork.Users.GetByIdWithRolesAsync(userId, cancellationToken);

        if (user is null)
            return Result.Failure<UserDetailVm>(UserErrors.NotFound(tokenValidation.UserId.Value));

        var roles = user.GetRoleNames();

        var dto = new UserDetailVm(
            user.Id.Value,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            user.Status.ToString(),
            user.IsEmailConfirmed,
            user.LastLoginAt,
            user.CreatedAt,
            roles
        );

        return Result.Success(dto);
    }
}
