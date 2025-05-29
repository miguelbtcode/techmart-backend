using Asp.Versioning;
using AuthMicroservice.Application.Features.EmailVerification.Commands.SendVerificationEmail;
using AuthMicroservice.Application.Features.EmailVerification.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroservice.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class EmailVerificationController : BaseApiController
{
    private readonly IMediator _mediator;

    public EmailVerificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("send")]
    [Authorize]
    public async Task<IActionResult> SendVerification()
    {
        var userId = GetCurrentUserId();
        var command = new SendVerificationEmailCommand(userId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        var command = new VerifyEmailCommand(request.Token);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = HttpContext.User.FindFirst("userId")?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}

public class VerifyEmailRequest
{
    public string Token { get; set; } = string.Empty;
}
