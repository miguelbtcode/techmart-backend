using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Commands.Login;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.API.Endpoints.Auth;

/// <summary>
/// Endpoint para autenticación de usuarios
/// </summary>
internal sealed class LoginEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Auth.Base)
            .WithTags(ApiTags.Authentication)
            .MapPost(ApiRoutes.Auth.Login, HandleAsync)
            .WithName("LoginUser")
            .WithSummary("Authenticate user credentials")
            .WithDescription("Authenticates user credentials and returns access tokens")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        ICommandHandler<LoginCommand, LoginVm> handler,
        HttpContext context
    )
    {
        var command = new LoginCommand(request.Email, request.Password, GetClientIp(context));

        var result = await handler.Handle(command, context.RequestAborted);

        return result.ToHttpResult(ApiMessages.UserLoggedIn);
    }

    /// <summary>
    /// Request model para login
    /// </summary>
    public sealed record LoginRequest(string Email, string Password);
}
