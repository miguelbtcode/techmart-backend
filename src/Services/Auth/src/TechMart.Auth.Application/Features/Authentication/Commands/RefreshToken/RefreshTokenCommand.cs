using TechMart.Auth.Application.Common.Results;
using TechMart.Auth.Application.Features.Authentication.Commands.Login;
using MediatR;

namespace TechMart.Auth.Application.Features.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;
