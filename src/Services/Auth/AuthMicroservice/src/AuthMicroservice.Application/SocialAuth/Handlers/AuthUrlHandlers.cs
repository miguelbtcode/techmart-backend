using MediatR;
using AuthMicroservice.Application.SocialAuth.Queries;
using AuthMicroservice.Domain.Interfaces;

namespace AuthMicroservice.Application.SocialAuth.Handlers;

public class GetGoogleAuthUrlHandler : IRequestHandler<GetGoogleAuthUrlQuery, string>
{
    private readonly ISocialAuthService _socialAuthService;

    public GetGoogleAuthUrlHandler(ISocialAuthService socialAuthService)
    {
        _socialAuthService = socialAuthService;
    }

    public async Task<string> Handle(GetGoogleAuthUrlQuery request, CancellationToken cancellationToken)
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

    public async Task<string> Handle(GetGitHubAuthUrlQuery request, CancellationToken cancellationToken)
    {
        return await _socialAuthService.GetGitHubAuthUrlAsync(request.RedirectUri);
    }
}
