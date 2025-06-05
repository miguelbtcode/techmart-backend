using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.EmailVerification.Commands.SendVerificationEmail;

public record SendVerificationEmailCommand(int UserId) : IRequest<Result>;
