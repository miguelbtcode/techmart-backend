using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.EmailVerification.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<Result>;
