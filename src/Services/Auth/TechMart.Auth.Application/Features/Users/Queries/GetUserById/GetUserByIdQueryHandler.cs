using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserByIdQuery, GetUserByIdQueryVm>
{
    public async Task<Result<GetUserByIdQueryVm>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.From(request.UserId);

        var user = await unitOfWork.Users.GetByIdWithRolesAsync(userId, cancellationToken);

        if (user is null)
            return Result.Failure<GetUserByIdQueryVm>(UserErrors.NotFound(request.UserId));

        var roles = user.GetRoleNames();

        var dto = new GetUserByIdQueryVm(
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
