using TechMart.Auth.Application.Abstractions.Messaging;

namespace TechMart.Auth.Application.Features.Users.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Token) : ICommand;
