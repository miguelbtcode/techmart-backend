using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.PasswordReset.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;
