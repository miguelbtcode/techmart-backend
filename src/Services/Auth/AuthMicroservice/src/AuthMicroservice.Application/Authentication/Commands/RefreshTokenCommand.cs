using AuthMicroservice.Application.Authentication.Commands;
using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Authentication.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;
