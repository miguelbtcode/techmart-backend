using MediatR;

namespace AuthMicroservice.Application.Features.SocialAuthentication.Queries;

public record GetGoogleAuthUrlQuery(string RedirectUri) : IRequest<string>;
