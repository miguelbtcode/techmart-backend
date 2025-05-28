using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Roles.Commands.AssignRole;

internal sealed class AssignRoleCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<AssignRoleCommand>
{
    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.From(request.UserId);
        var roleId = RoleId.From(request.RoleId);

        // Verify user exists
        var userExists = await unitOfWork.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        if (!userExists)
            return UserErrors.NotFound(userId);

        // Verify role exists
        var roleExists = await unitOfWork.Roles.AnyAsync(r => r.Id == roleId, cancellationToken);
        if (!roleExists)
            return Error.NotFound("Role.NotFound", $"Role with ID '{roleId}' was not found");

        // Check if user already has this role
        var existingUserRole = await unitOfWork.UserRoles.GetUserRoleAsync(
            userId,
            roleId,
            cancellationToken
        );
        if (existingUserRole != null)
            return Error.Conflict("UserRole.AlreadyExists", "User already has this role assigned");

        // Create user role assignment
        var userRoleResult = UserRole.Create(userId, roleId, request.AssignedBy);
        if (userRoleResult.IsFailure)
            return userRoleResult.Error;

        unitOfWork.Repository<UserRole, UserRoleId>().Add(userRoleResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.DispatchDomainEventsAsync(cancellationToken);

        return Result.Success();
    }
}
