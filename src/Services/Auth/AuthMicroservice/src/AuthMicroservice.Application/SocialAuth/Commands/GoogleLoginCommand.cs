using AuthMicroservice.Application.Authentication.Commands;
using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.SocialAuth.Commands;

public record GoogleLoginCommand(string AccessToken, bool RememberMe = false)
    : IRequest<Result<LoginResponse>>;
