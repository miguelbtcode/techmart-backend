using TechMart.Auth.Application.Abstractions.Messaging;

namespace TechMart.Auth.Application.Features.Users.Commands.ChangePassword;

public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword)
    : ICommand;
