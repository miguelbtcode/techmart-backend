using AuthMicroservice.Models.DTOs;
using AuthMicroservice.Services;
using FastEndpoints;

namespace AuthMicroservice.Controllers;

public class LogoutEndpoint : Endpoint<RefreshTokenRequest>
{
    private readonly IAuthService _authService;

    public LogoutEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/logout");
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Cerrar sesión";
            s.Description = "Revoca el refresh token del usuario";
        });
    }

    public override async Task HandleAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var success = await _authService.LogoutAsync(request.RefreshToken);

        if (success)
        {
            await SendOkAsync(new { message = "Sesión cerrada exitosamente" }, ct);
        }
        else
        {
            await SendAsync(new { message = "Error al cerrar sesión" }, 400, ct);
        }
    }
}
