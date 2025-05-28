using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Roles.Commands.RemoveRole;

internal sealed class RemoveRoleCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveRoleCommand>
{
    public async Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.From(request.UserId);
        var roleId = RoleId.From(request.RoleId);

        // Find the user role assignment
        var userRole = await unitOfWork.UserRoles.GetUserRoleAsync(
            userId,
            roleId,
            cancellationToken
        );
        if (userRole == null)
            return Error.NotFound("UserRole.NotFound", "User role assignment not found");

        // Check if this is the last role (business rule: user must have at least one role)
        var userRoleCount = await unitOfWork.UserRoles.CountAsync(
            ur => ur.UserId == userId,
            cancellationToken
        );
        if (userRoleCount <= 1)
            return Error.Validation(
                "UserRole.CannotRemoveLastRole",
                "Cannot remove the last role from user"
            );

        // Remove the role assignment
        userRole.Remove(request.RemovedBy);
        unitOfWork.Repository<UserRole, UserRoleId>().Remove(userRole);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.DispatchDomainEventsAsync(cancellationToken);

        return Result.Success();
    }
}
