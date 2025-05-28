using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Users.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand;
