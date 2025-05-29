using MediatR;

namespace AuthMicroservice.Application.SocialAuth.Queries;

public record GetGitHubAuthUrlQuery(string RedirectUri) : IRequest<string>;
