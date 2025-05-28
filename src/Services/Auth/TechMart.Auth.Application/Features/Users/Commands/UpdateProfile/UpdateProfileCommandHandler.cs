using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Errors;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Commands.UpdateProfile;

internal sealed class UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProfileCommand>
{
    public async Task<Result> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.From(request.UserId);
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            return UserErrors.NotFound(request.UserId);

        var updateResult = user.UpdateProfile(request.FirstName, request.LastName, request.UserId);

        if (updateResult.IsFailure)
            return updateResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
