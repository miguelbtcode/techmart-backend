using AuthMicroservice.Models.DTOs;
using AuthMicroservice.Services;
using AuthMicroservice.Validators;
using FastEndpoints;

namespace AuthMicroservice.Controllers;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuthService _authService;

    public LoginEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Validator<LoginRequestValidator>();

        Summary(s =>
        {
            s.Summary = "Iniciar sesión";
            s.Description = "Autentica al usuario y devuelve tokens JWT";
            s.ExampleRequest = new LoginRequest
            {
                EmailOrUsername = "usuario@ejemplo.com",
                Password = "MiPassword123",
                RememberMe = false,
            };
        });
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request);

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
