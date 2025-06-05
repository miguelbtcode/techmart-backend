using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.PasswordReset.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;
