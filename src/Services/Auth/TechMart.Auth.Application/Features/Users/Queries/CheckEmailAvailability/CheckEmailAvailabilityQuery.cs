using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Users.Queries.CheckEmailAvailability;

public sealed record CheckEmailAvailabilityQuery(string Email, Guid? ExcludeUserId = null)
    : IQuery<CheckEmailAvailabilityVm>;
