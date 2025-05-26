using TechMart.Auth.Application.Abstractions.Messaging;

namespace TechMart.Auth.Application.Features.Users.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand;
