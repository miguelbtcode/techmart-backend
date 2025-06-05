using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.PasswordReset.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
