using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.Authentication.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result>;
