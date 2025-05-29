using MediatR;

namespace AuthMicroservice.Domain.Events;

public record UserRegisteredEvent(
    int UserId,
    string Email,
    string Username,
    DateTime OccurredAt
) : INotification;
