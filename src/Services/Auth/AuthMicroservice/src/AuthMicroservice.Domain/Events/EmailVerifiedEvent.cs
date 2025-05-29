using MediatR;

namespace AuthMicroservice.Domain.Events;

public record EmailVerifiedEvent(
    int UserId,
    string Email,
    DateTime OccurredAt
) : INotification;
