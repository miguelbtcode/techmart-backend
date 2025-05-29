using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.Features.Authentication.Commands.Login;
using MediatR;

namespace AuthMicroservice.Application.Features.SocialAuthentication.Commands.GitHubLogin;

public record GitHubLoginCommand(string AccessToken, bool RememberMe = false)
    : IRequest<Result<LoginResponse>>;
