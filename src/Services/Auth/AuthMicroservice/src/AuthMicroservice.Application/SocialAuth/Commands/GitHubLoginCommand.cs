using MediatR;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.Authentication.Commands;

namespace AuthMicroservice.Application.SocialAuth.Commands;

public record GitHubLoginCommand(
    string AccessToken,
    bool RememberMe = false
) : IRequest<Result<LoginResponse>>;
