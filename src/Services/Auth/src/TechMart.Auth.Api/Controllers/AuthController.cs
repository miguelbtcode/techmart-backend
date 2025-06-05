using Asp.Versioning;
using TechMart.Auth.Application.Features.Authentication.Commands.Login;
using TechMart.Auth.Application.Features.Authentication.Commands.Logout;
using TechMart.Auth.Application.Features.Authentication.Commands.RefreshToken;
using TechMart.Auth.Application.Features.UserManagement.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TechMart.Auth.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Username,
            request.Password,
            request.FirstName,
            request.LastName
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(
            request.EmailOrUsername,
            request.Password,
            request.RememberMe,
            GetClientIpAddress(),
            GetUserAgent()
        );

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return Unauthorized(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return Unauthorized(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        var command = new LogoutCommand(request.RefreshToken);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgent()
    {
        return HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
    }
}

// Request DTOs
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class LoginRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
