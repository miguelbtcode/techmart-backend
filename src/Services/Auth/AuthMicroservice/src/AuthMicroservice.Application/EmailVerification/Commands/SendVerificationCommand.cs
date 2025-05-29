using MediatR;
using AuthMicroservice.Application.Common.Results;

namespace AuthMicroservice.Application.EmailVerification.Commands;

public record SendVerificationCommand(int UserId) : IRequest<Result>;
