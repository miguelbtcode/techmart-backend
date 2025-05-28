using TechMart.Auth.Application.Abstractions.Authentication;
using TechMart.Auth.Application.Abstractions.Contracts;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Commands.Login;

internal sealed class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IRefreshTokenService refreshTokenService
) : ICommandHandler<LoginCommand, LoginVm>
{
    public async Task<Result<LoginVm>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        // Create email value object
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<LoginVm>(emailResult.Error);

        // Get user by email with roles
        var user = await unitOfWork.Users.GetByEmailWithRolesAsync(
            emailResult.Value,
            cancellationToken
        );

        if (user is null)
            return Result.Failure<LoginVm>(UserErrors.InvalidCredentials());

        // Verify password
        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result.Failure<LoginVm>(UserErrors.InvalidCredentials());

        // Check if user can login
        if (!user.CanLogin())
        {
            if (!user.IsEmailConfirmed)
                return Result.Failure<LoginVm>(UserErrors.EmailNotConfirmed());

            return Result.Failure<LoginVm>(UserErrors.UserNotActive());
        }

        // Record login
        var loginResult = user.RecordLogin(request.IpAddress);
        if (loginResult.IsFailure)
            return Result.Failure<LoginVm>(loginResult.Error);

        // Generate tokens
        var accessToken = jwtProvider.GenerateToken(user);
        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user);

        // Save changes (for LastLoginAt update)
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.DispatchDomainEventsAsync(cancellationToken);

        // var roles = user..Select(ur => ur.Role?.Name ?? "Unknown");
        var roles = user.GetRoleNames();

        return Result.Success(
            new LoginVm(
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddHours(1), // Configure this from settings
                new UserInfoVm(
                    user.Id.Value,
                    user.Email.Value,
                    user.FirstName,
                    user.LastName,
                    roles
                )
            )
        );
    }
}
