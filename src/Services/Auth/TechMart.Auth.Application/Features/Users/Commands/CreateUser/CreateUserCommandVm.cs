namespace TechMart.Auth.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommandVm(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName
);
