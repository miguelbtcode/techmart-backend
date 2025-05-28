using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Users.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(Guid UserId, string FirstName, string LastName)
    : ICommand;
