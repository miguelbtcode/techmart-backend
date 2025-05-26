using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Queries.CheckEmailAvailability;

internal sealed class CheckEmailAvailabilityQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<CheckEmailAvailabilityQuery, EmailAvailabilityVm>
{
    public async Task<Result<EmailAvailabilityVm>> Handle(
        CheckEmailAvailabilityQuery request,
        CancellationToken cancellationToken
    )
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<EmailAvailabilityVm>(emailResult.Error);

        var excludeUserId = request.ExcludeUserId.HasValue
            ? UserId.From(request.ExcludeUserId.Value)
            : null;

        var isAvailable = await unitOfWork.Users.IsEmailUniqueAsync(
            emailResult.Value,
            excludeUserId,
            cancellationToken
        );

        var dto = new EmailAvailabilityVm(isAvailable, request.Email);

        return Result.Success(dto);
    }
}
