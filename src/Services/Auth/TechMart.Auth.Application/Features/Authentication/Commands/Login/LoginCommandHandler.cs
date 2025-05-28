using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Application.Features.Shared.Dtos;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Login;

internal sealed class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IRefreshTokenService refreshTokenService
) : ICommandHandler<LoginCommand, LoginCommandVm>
{
    public async Task<Result<LoginCommandVm>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        // Create email value object
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<LoginCommandVm>(emailResult.Error);

        // Get user by email with roles
        var user = await unitOfWork.Users.GetByEmailWithRolesAsync(
            emailResult.Value,
            cancellationToken
        );

        if (user is null)
            return Result.Failure<LoginCommandVm>(UserErrors.InvalidCredentials());

        // Verify password
        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result.Failure<LoginCommandVm>(UserErrors.InvalidCredentials());

        // Check if user can login
        if (!user.CanLogin())
        {
            if (!user.IsEmailConfirmed)
                return Result.Failure<LoginCommandVm>(UserErrors.EmailNotConfirmed());

            return Result.Failure<LoginCommandVm>(UserErrors.UserNotActive());
        }

        // Record login
        var loginResult = user.RecordLogin(request.IpAddress);
        if (loginResult.IsFailure)
            return Result.Failure<LoginCommandVm>(loginResult.Error);

        // Generate tokens
        var accessToken = jwtProvider.GenerateToken(user);
        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(
            user,
            cancellationToken
        );

        // Save changes (for LastLoginAt update)
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.DispatchDomainEventsAsync(cancellationToken);

        var roles = user.GetRoleNames();

        var userInfo = new UserInfoVm(
            user.Id.Value,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            roles
        );

        return Result.Success(
            new LoginCommandVm(
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddHours(1), // Configure this from settings
                userInfo
            )
        );
    }
}
