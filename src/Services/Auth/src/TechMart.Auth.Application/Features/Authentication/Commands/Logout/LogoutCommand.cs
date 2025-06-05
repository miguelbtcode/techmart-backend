using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.Authentication.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result>;
