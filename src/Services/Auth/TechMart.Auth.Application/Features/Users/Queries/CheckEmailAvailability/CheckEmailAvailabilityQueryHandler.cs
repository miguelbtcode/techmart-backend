using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Application.Features.Users.Queries.CheckEmailAvailability;

internal sealed class CheckEmailAvailabilityQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<CheckEmailAvailabilityQuery, CheckEmailAvailabilityVm>
{
    public async Task<Result<CheckEmailAvailabilityVm>> Handle(
        CheckEmailAvailabilityQuery request,
        CancellationToken cancellationToken
    )
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<CheckEmailAvailabilityVm>(emailResult.Error);

        var excludeUserId = request.ExcludeUserId.HasValue
            ? UserId.From(request.ExcludeUserId.Value)
            : null;

        var isAvailable = await unitOfWork.Users.IsEmailUniqueAsync(
            emailResult.Value,
            excludeUserId,
            cancellationToken
        );

        var dto = new CheckEmailAvailabilityVm(isAvailable, request.Email);

        return Result.Success(dto);
    }
}
