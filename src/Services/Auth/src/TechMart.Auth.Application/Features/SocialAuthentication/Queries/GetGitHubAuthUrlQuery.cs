using MediatR;

namespace TechMart.Auth.Application.Features.SocialAuthentication.Queries;

public record GetGitHubAuthUrlQuery(string RedirectUri) : IRequest<string>;
