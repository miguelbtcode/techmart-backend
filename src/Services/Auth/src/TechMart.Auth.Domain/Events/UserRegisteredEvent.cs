using MediatR;

namespace TechMart.Auth.Domain.Events;

public record UserRegisteredEvent(
    int UserId,
    string Email,
    string Username,
    DateTime OccurredAt
) : INotification;
