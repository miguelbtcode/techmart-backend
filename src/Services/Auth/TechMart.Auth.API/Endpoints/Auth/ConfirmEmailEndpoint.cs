using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Features.Users.Commands.ConfirmEmail;
using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.API.Endpoints.Auth;

/// <summary>
/// Endpoint para confirmación de email
/// </summary>
internal sealed class ConfirmEmailEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Auth.Base)
            .WithTags(ApiTags.Authentication)
            .MapPost(ApiRoutes.Auth.ConfirmEmail, HandleAsync)
            .WithName("ConfirmEmail")
            .WithSummary("Confirm user email address")
            .WithDescription("Confirms a user's email address using the provided token")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ConfirmEmailRequest request,
        ICommandHandler<ConfirmEmailCommand> handler,
        HttpContext context
    )
    {
        var command = new ConfirmEmailCommand(request.Email, request.Token);
        var result = await handler.Handle(command, context.RequestAborted);

        return result.ToHttpResult(ApiMessages.EmailConfirmed);
    }

    /// <summary>
    /// Request model para confirmación de email
    /// </summary>
    public sealed record ConfirmEmailRequest(string Email, string Token);
}
