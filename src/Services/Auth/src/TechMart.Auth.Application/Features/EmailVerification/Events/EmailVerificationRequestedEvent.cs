using MediatR;

namespace TechMart.Auth.Application.Features.EmailVerification.Events;

public record EmailVerificationRequestedEvent(
    int UserId,
    string Email,
    string Token,
    DateTime OccurredAt
) : INotification;
