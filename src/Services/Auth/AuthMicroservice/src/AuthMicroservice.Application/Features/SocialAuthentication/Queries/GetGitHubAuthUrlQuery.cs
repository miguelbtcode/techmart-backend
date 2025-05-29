using MediatR;

namespace AuthMicroservice.Application.Features.SocialAuthentication.Queries;

public record GetGitHubAuthUrlQuery(string RedirectUri) : IRequest<string>;
