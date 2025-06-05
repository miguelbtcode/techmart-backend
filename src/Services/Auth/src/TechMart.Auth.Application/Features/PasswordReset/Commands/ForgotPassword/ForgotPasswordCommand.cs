using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.PasswordReset.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
