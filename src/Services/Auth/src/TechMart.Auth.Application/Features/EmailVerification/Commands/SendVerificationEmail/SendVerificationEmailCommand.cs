using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.EmailVerification.Commands.SendVerificationEmail;

public record SendVerificationEmailCommand(int UserId) : IRequest<Result>;
