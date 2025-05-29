using AuthMicroservice.Models.DTOs;
using AuthMicroservice.Services;
using AuthMicroservice.Validators;
using FastEndpoints;

namespace AuthMicroservice.Controllers;

public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, RefreshTokenResponse>
{
    private readonly IAuthService _authService;

    public RefreshTokenEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/refresh");
        AllowAnonymous();
        Validator<RefreshTokenRequestValidator>();

        Summary(s =>
        {
            s.Summary = "Renovar token de acceso";
            s.Description = "Genera un nuevo access token usando el refresh token";
            s.ExampleRequest = new RefreshTokenRequest
            {
                RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
            };
        });
    }

    public override async Task HandleAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (result.Success)
        {
            await SendOkAsync(result, ct);
        }
        else
        {
            await SendAsync(result, 401, ct);
        }
    }
}
