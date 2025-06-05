using MediatR;

namespace TechMart.Auth.Application.Features.SocialAuthentication.Queries;

public record GetGoogleAuthUrlQuery(string RedirectUri) : IRequest<string>;
