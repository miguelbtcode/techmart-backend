using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher
) : ICommandHandler<CreateUserCommand, CreateUserCommandVm>
{
    public async Task<Result<CreateUserCommandVm>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<CreateUserCommandVm>(emailResult.Error);

        var email = emailResult.Value;

        // Check if email is unique
        var isUnique = await unitOfWork.Users.IsEmailUniqueAsync(email, null, cancellationToken);
        if (!isUnique)
            return Result.Failure<CreateUserCommandVm>(
                UserErrors.EmailAlreadyExists(request.Email)
            );

        // Validate password
        if (request.Password != request.ConfirmPassword)
            return Result.Failure<CreateUserCommandVm>(UserErrors.PasswordsDoNotMatch());

        var passwordResult = Password.Create(request.Password);
        if (passwordResult.IsFailure)
            return Result.Failure<CreateUserCommandVm>(passwordResult.Error);

        var password = passwordResult.Value;

        // Hash password
        var passwordHash = passwordHasher.HashPassword(password.Value);

        // Create user
        var userResult = User.Create(email, request.FirstName, request.LastName, passwordHash);
        if (userResult.IsFailure)
            return Result.Failure<CreateUserCommandVm>(userResult.Error);

        var user = userResult.Value;
        unitOfWork.Repository<User, UserId>().Add(user);

        // Assign default customer role
        var customerRole = await unitOfWork.Roles.GetByIdAsync(RoleId.Customer, cancellationToken);
        if (customerRole is not null)
        {
            var userRoleResult = UserRole.Create(
                user.Id,
                customerRole.Id,
                null,
                isInitialAssignment: true
            );

            if (userRoleResult.IsSuccess)
            {
                unitOfWork.Repository<UserRole, UserRoleId>().Add(userRoleResult.Value);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.DispatchDomainEventsAsync(cancellationToken);

        return Result.Success(
            new CreateUserCommandVm(user.Id.Value, user.Email.Value, user.FirstName, user.LastName)
        );
    }
}
