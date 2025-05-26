using TechMart.Auth.Application.Abstractions.Authentication;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Roles.Entities;
using TechMart.Auth.Domain.Roles.ValueObjects;
using TechMart.Auth.Domain.Users.Entities;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher
) : ICommandHandler<RegisterUserCommand, RegisterUserVm>
{
    public async Task<Result<RegisterUserVm>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        // Create email value object
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<RegisterUserVm>(emailResult.Error);

        var email = emailResult.Value;

        // Check if email is unique
        var isUnique = await unitOfWork.Users.IsEmailUniqueAsync(email, null, cancellationToken);

        if (!isUnique)
            return Result.Failure<RegisterUserVm>(UserErrors.EmailAlreadyExists(request.Email));

        var passwordResult = Password.Create(request.Password);

        if (passwordResult.IsFailure)
            return Result.Failure<RegisterUserVm>(passwordResult.Error);

        var password = passwordResult.Value;

        // Hash password
        var passwordHash = passwordHasher.HashPassword(password.Value);

        // Create user
        var userResult = User.Create(email, request.FirstName, request.LastName, passwordHash);

        if (userResult.IsFailure)
            return Result.Failure<RegisterUserVm>(userResult.Error);

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
            new RegisterUserVm(user.Id.Value, user.Email.Value, user.FirstName, user.LastName)
        );
    }
}
