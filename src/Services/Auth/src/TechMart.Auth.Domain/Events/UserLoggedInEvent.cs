using MediatR;

namespace AuthMicroservice.Domain.Events;

public record UserLoggedInEvent(
    int UserId,
    string Email,
    string? IpAddress,
    string? UserAgent,
    DateTime OccurredAt
) : INotification;
