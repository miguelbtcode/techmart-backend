using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.Application.Features.Authentication.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenCommandVm>;
