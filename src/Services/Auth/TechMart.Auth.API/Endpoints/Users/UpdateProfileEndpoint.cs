using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Commands.UpdateProfile;

namespace TechMart.Auth.API.Endpoints.Users;

/// <summary>
/// Endpoint para actualizar perfil de usuario
/// </summary>
internal sealed class UpdateProfileEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Users.Base)
            .WithTags(ApiTags.Users)
            .MapPut(ApiRoutes.Users.Profile, HandleAsync)
            .WithName("UpdateUserProfile")
            .WithSummary("Update user profile")
            .WithDescription("Updates the authenticated user's profile information")
            .RequireAuthorization()
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized);
    }

    [Authorize]
    private static async Task<IResult> HandleAsync(
        UpdateProfileRequest request,
        ICommandHandler<UpdateProfileCommand> handler,
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

        var command = new UpdateProfileCommand(userId, request.FirstName, request.LastName);
        var result = await handler.Handle(command, context.RequestAborted);

        return result.ToHttpResult(ApiMessages.UserProfileUpdated);
    }

    /// <summary>
    /// Request model para actualización de perfil
    /// </summary>
    public sealed record UpdateProfileRequest(string FirstName, string LastName);
}
