using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserByIdQuery, UserDetailVm>
{
    public async Task<Result<UserDetailVm>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.From(request.UserId);

        var user = await unitOfWork.Users.GetByIdWithRolesAsync(userId, cancellationToken);

        if (user is null)
            return Result.Failure<UserDetailVm>(UserErrors.NotFound(request.UserId));

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
