using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.Application.Features.Users.Queries.CheckEmailAvailability;

public sealed record CheckEmailAvailabilityQuery(string Email, Guid? ExcludeUserId = null)
    : IQuery<EmailAvailabilityVm>;
