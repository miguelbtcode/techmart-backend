using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Login;

public sealed record LoginCommand(string Email, string Password, string? IpAddress)
    : ICommand<LoginCommandVm>;
