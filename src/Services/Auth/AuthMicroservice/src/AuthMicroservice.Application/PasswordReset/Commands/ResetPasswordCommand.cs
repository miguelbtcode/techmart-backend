using MediatR;
using AuthMicroservice.Application.Common.Results;

namespace AuthMicroservice.Application.PasswordReset.Commands;

public record ResetPasswordCommand(
    string Token,
    string NewPassword
) : IRequest<Result>;
