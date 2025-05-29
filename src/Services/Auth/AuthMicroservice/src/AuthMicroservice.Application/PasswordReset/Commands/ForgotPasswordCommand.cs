using MediatR;
using AuthMicroservice.Application.Common.Results;

namespace AuthMicroservice.Application.PasswordReset.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
