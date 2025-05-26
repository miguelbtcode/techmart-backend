using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserByEmail;

internal sealed class GetUserByEmailQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserByEmailQuery, UserDetailVm>
{
    public async Task<Result<UserDetailVm>> Handle(
        GetUserByEmailQuery request,
        CancellationToken cancellationToken
    )
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<UserDetailVm>(emailResult.Error);

        var user = await unitOfWork.Users.GetByEmailWithRolesAsync(
            emailResult.Value,
            cancellationToken
        );

        if (user is null)
            return Result.Failure<UserDetailVm>(UserErrors.NotFoundByEmail(request.Email));

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
