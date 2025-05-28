using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Logout;

public sealed record LogoutCommand(Guid UserId) : ICommand;
