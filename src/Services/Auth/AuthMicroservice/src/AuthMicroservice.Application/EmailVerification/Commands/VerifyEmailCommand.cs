using MediatR;
using AuthMicroservice.Application.Common.Results;

namespace AuthMicroservice.Application.EmailVerification.Commands;

public record VerifyEmailCommand(string Token) : IRequest<Result>;
