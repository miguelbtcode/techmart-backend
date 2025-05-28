using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Authentication.Queries.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler(IUnitOfWork unitOfWork, IJwtProvider jwtProvider)
    : IQueryHandler<GetCurrentUserQuery, GetCurrentUserQueryVm>
{
    public async Task<Result<GetCurrentUserQueryVm>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken
    )
    {
        // Validate and decode token
        var tokenValidation = await jwtProvider.ValidateAccessTokenAsync(
            request.AccessToken,
            cancellationToken
        );

        if (!tokenValidation.IsValid || tokenValidation.UserId is null)
            return Result.Failure<GetCurrentUserQueryVm>(
                Error.Unauthorized("Token.Invalid", "Token is invalid or expired")
            );

        var userId = UserId.From(tokenValidation.UserId.Value);

        var user = await unitOfWork.Users.GetByIdWithRolesAsync(userId, cancellationToken);

        if (user is null)
            return Result.Failure<GetCurrentUserQueryVm>(
                UserErrors.NotFound(tokenValidation.UserId.Value)
            );

        var roles = user.GetRoleNames();

        var dto = new GetCurrentUserQueryVm(
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
