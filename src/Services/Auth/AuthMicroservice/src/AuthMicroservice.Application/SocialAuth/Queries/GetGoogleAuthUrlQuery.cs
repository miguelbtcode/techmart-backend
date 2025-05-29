using MediatR;

namespace AuthMicroservice.Application.SocialAuth.Queries;

public record GetGoogleAuthUrlQuery(string RedirectUri) : IRequest<string>;
