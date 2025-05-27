using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Commands.ResetPassword;

namespace TechMart.Auth.API.Endpoints.Auth;

/// <summary>
/// Endpoint para restablecer contraseña
/// </summary>
internal sealed class ResetPasswordEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Auth.Base)
            .WithTags(ApiTags.Authentication)
            .MapPost(ApiRoutes.Auth.ResetPassword, HandleAsync)
            .WithName("ResetPassword")
            .WithSummary("Reset user password")
            .WithDescription("Resets the user's password using the provided token")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ResetPasswordRequest request,
        ICommandHandler<ResetPasswordCommand> handler,
        HttpContext context
    )
    {
        var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);
        var result = await handler.Handle(command, context.RequestAborted);

        return result.ToHttpResult(ApiMessages.PasswordReset);
    }

    /// <summary>
    /// Request model para restablecimiento de contraseña
    /// </summary>
    public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);
}
