using Asp.Versioning;
using AuthMicroservice.Application.Features.PasswordReset.Commands.ForgotPassword;
using AuthMicroservice.Application.Features.PasswordReset.Commands.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroservice.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PasswordResetController : BaseApiController
{
    private readonly IMediator _mediator;

    public PasswordResetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await _mediator.Send(command);

        // Always return success for security (dont reveal if email exists)
        return Ok(new { message = "If the email exists, you will receive reset instructions." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(request.Token, request.NewPassword);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
