using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserByEmail;

internal sealed class GetUserByEmailQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserByEmailQuery, GetUserByEmailQueryVm>
{
    public async Task<Result<GetUserByEmailQueryVm>> Handle(
        GetUserByEmailQuery request,
        CancellationToken cancellationToken
    )
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<GetUserByEmailQueryVm>(emailResult.Error);

        var user = await unitOfWork.Users.GetByEmailWithRolesAsync(
            emailResult.Value,
            cancellationToken
        );

        if (user is null)
            return Result.Failure<GetUserByEmailQueryVm>(UserErrors.NotFoundByEmail(request.Email));

        var roles = user.GetRoleNames();

        var dto = new GetUserByEmailQueryVm(
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
