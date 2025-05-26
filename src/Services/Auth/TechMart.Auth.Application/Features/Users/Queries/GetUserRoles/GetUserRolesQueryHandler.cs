using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserRoles;

internal sealed class GetUserRolesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserRolesQuery, UserRolesVm>
{
    public async Task<Result<UserRolesVm>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.From(request.UserId);

        var userExists = await unitOfWork.Users.AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExists)
            return Result.Failure<UserRolesVm>(UserErrors.NotFound(request.UserId));

        var userRoles = await unitOfWork.UserRoles.GetUserRolesAsync(userId, cancellationToken);

        var roles = userRoles.Select(ur => new RoleVm(
            ur.RoleId.Value,
            ur.Role?.Name ?? "Unknown",
            ur.Role?.Description ?? "",
            ur.Role?.HierarchyLevel ?? 0,
            ur.AssignedAt
        ));

        var vm = new UserRolesVm(request.UserId, roles);

        return Result.Success(vm);
    }
}
