using MediatR;

namespace TechMart.Auth.Domain.Events;

public record EmailVerifiedEvent(
    int UserId,
    string Email,
    DateTime OccurredAt
) : INotification;
