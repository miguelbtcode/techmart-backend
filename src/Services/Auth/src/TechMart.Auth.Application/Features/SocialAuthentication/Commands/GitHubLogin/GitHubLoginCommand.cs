using TechMart.Auth.Application.Common.Results;
using TechMart.Auth.Application.Features.Authentication.Commands.Login;
using MediatR;

namespace TechMart.Auth.Application.Features.SocialAuthentication.Commands.GitHubLogin;

public record GitHubLoginCommand(string AccessToken, bool RememberMe = false)
    : IRequest<Result<LoginResponse>>;
