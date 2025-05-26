namespace TechMart.Auth.Application.Features.Users.Vms;

public sealed record RegisterUserVm(Guid UserId, string Email, string FirstName, string LastName);
