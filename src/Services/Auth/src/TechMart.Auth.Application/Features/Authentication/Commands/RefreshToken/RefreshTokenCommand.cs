using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.Features.Authentication.Commands.Login;
using MediatR;

namespace AuthMicroservice.Application.Features.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;
