using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.EmailVerification.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<Result>;
