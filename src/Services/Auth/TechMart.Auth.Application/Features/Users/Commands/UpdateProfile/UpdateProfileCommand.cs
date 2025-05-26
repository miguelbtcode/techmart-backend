using TechMart.Auth.Application.Abstractions.Messaging;

namespace TechMart.Auth.Application.Features.Users.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(Guid UserId, string FirstName, string LastName)
    : ICommand;
