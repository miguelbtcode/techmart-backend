using TechMart.Auth.Application.Abstractions.Contracts;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Commands.ConfirmEmail;

internal sealed class ConfirmEmailCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailConfirmationService emailConfirmationService
) : ICommandHandler<ConfirmEmailCommand>
{
    public async Task<Result> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate token
        var isValidToken = await emailConfirmationService.ValidateTokenAsync(
            request.Email,
            request.Token,
            cancellationToken
        );

        if (!isValidToken)
            return UserErrors.InvalidTokenEmailConfirm();

        // Get user
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var user = await unitOfWork.Users.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
            return UserErrors.NotFoundByEmail(request.Email);

        // Confirm email
        var confirmResult = user.ConfirmEmail();
        if (confirmResult.IsFailure)
            return confirmResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.DispatchDomainEventsAsync(cancellationToken);

        return Result.Success();
    }
}
