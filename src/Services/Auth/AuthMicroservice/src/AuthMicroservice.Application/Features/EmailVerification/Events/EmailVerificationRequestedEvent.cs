using MediatR;

namespace AuthMicroservice.Application.Features.EmailVerification.Events;

public record EmailVerificationRequestedEvent(
    int UserId,
    string Email,
    string Token,
    DateTime OccurredAt
) : INotification;
