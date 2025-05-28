using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName
) : ICommand<RegisterUserVm>;
