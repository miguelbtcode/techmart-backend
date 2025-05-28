using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName
) : ICommand<CreateUserCommandVm>;
