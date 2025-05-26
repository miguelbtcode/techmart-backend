using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.Application.Features.Users.Commands.Login;

public sealed record LoginCommand(string Email, string Password, string? IpAddress)
    : ICommand<LoginVm>;
