using Asp.Versioning;
using AuthMicroservice.Application.SocialAuth.Commands;
using AuthMicroservice.Application.SocialAuth.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroservice.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SocialAuthController : BaseApiController
{
    private readonly IMediator _mediator;

    public SocialAuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] SocialLoginRequest request)
    {
        var command = new GoogleLoginCommand(request.AccessToken, request.RememberMe);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("github")]
    public async Task<IActionResult> GitHubLogin([FromBody] SocialLoginRequest request)
    {
        var command = new GitHubLoginCommand(request.AccessToken, request.RememberMe);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("google/url")]
    public async Task<IActionResult> GetGoogleAuthUrl([FromQuery] string redirectUri)
    {
        var query = new GetGoogleAuthUrlQuery(redirectUri);
        var result = await _mediator.Send(query);
        return Ok(new { authUrl = result });
    }

    [HttpGet("github/url")]
    public async Task<IActionResult> GetGitHubAuthUrl([FromQuery] string redirectUri)
    {
        var query = new GetGitHubAuthUrlQuery(redirectUri);
        var result = await _mediator.Send(query);
        return Ok(new { authUrl = result });
    }
}

public class SocialLoginRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}
