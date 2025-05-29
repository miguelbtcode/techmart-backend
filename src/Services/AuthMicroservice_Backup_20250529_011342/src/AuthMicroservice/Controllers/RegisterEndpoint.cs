using AuthMicroservice.Models.DTOs;
using AuthMicroservice.Services;
using AuthMicroservice.Validators;
using FastEndpoints;

namespace AuthMicroservice.Controllers;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly IAuthService _authService;

    public RegisterEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
        Validator<RegisterRequestValidator>();

        Summary(s =>
        {
            s.Summary = "Registrar nuevo usuario";
            s.Description = "Crea una nueva cuenta de usuario en el sistema";
            s.ExampleRequest = new RegisterRequest
            {
                Email = "usuario@ejemplo.com",
                Username = "usuario123",
                Password = "MiPassword123",
                ConfirmPassword = "MiPassword123",
                FirstName = "Juan",
                LastName = "Pérez",
            };
        });
    }

    public override async Task HandleAsync(RegisterRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request);

        if (result.Success)
        {
            await SendOkAsync(result, ct);
        }
        else
        {
            await SendAsync(result, 400, ct);
        }
    }
}
