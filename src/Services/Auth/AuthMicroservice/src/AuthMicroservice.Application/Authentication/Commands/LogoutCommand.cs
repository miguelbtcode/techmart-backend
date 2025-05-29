using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Authentication.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<Result>;
