using TechMart.Auth.Application.Abstractions.Messaging;

namespace TechMart.Auth.Application.Features.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword)
    : ICommand;
