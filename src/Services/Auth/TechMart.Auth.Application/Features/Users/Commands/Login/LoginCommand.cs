using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Users.Commands.Login;

public sealed record LoginCommand(string Email, string Password, string? IpAddress)
    : ICommand<LoginVm>;
