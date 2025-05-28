using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword)
    : ICommand;
