using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Commands.ForgotPassword;

namespace TechMart.Auth.API.Endpoints.Auth;

/// <summary>
/// Endpoint para solicitar restablecimiento de contraseña
/// </summary>
internal sealed class ForgotPasswordEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Auth.Base)
            .WithTags(ApiTags.Authentication)
            .MapPost(ApiRoutes.Auth.ForgotPassword, HandleAsync)
            .WithName("ForgotPassword")
            .WithSummary("Request password reset")
            .WithDescription("Sends a password reset email to the user")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ForgotPasswordRequest request,
        ICommandHandler<ForgotPasswordCommand> handler,
        HttpContext context
    )
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await handler.Handle(command, context.RequestAborted);

        return result.ToHttpResult(ApiMessages.PasswordResetSent);
    }

    /// <summary>
    /// Request model para olvido de contraseña
    /// </summary>
    public sealed record ForgotPasswordRequest(string Email);
}
