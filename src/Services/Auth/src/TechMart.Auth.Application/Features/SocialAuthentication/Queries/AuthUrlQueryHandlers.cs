using TechMart.Auth.Domain.Interfaces;
using MediatR;

namespace TechMart.Auth.Application.Features.SocialAuthentication.Queries;

public class GetGoogleAuthUrlHandler : IRequestHandler<GetGoogleAuthUrlQuery, string>
{
    private readonly ISocialAuthService _socialAuthService;

    public GetGoogleAuthUrlHandler(ISocialAuthService socialAuthService)
    {
        _socialAuthService = socialAuthService;
    }

    public async Task<string> Handle(
        GetGoogleAuthUrlQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _socialAuthService.GetGoogleAuthUrlAsync(request.RedirectUri);
    }
}

public class GetGitHubAuthUrlHandler : IRequestHandler<GetGitHubAuthUrlQuery, string>
{
    private readonly ISocialAuthService _socialAuthService;

    public GetGitHubAuthUrlHandler(ISocialAuthService socialAuthService)
    {
        _socialAuthService = socialAuthService;
    }

    public async Task<string> Handle(
        GetGitHubAuthUrlQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _socialAuthService.GetGitHubAuthUrlAsync(request.RedirectUri);
    }
}
