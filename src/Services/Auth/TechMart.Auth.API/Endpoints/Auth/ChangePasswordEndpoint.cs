using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Commands.ChangePassword;

namespace TechMart.Auth.API.Endpoints.Auth;

/// <summary>
/// Endpoint para cambiar contraseña
/// </summary>
internal sealed class ChangePasswordEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Auth.Base)
            .WithTags(ApiTags.Authentication)
            .MapPost(ApiRoutes.Auth.ChangePassword, HandleAsync)
            .WithName("ChangePassword")
            .WithSummary("Change user password")
            .WithDescription("Changes the authenticated user's password")
            .RequireAuthorization()
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized);
    }

    [Authorize]
    private static async Task<IResult> HandleAsync(
        ChangePasswordRequest request,
        ICommandHandler<ChangePasswordCommand> handler,
        HttpContext context
    )
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Json(
                ApiResponse.Failure(ApiMessages.InvalidCredentials),
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        var command = new ChangePasswordCommand(
            userId,
            request.CurrentPassword,
            request.NewPassword
        );
        var result = await handler.Handle(command, context.RequestAborted);

        return result.ToHttpResult(ApiMessages.PasswordChanged);
    }

    /// <summary>
    /// Request model para cambio de contraseña
    /// </summary>
    public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
