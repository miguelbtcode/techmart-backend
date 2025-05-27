using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Commands.RegisterUser;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.API.Endpoints.Auth;

/// <summary>
/// Endpoint para registro de usuarios
/// </summary>
internal sealed class RegisterEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Auth.Base)
            .WithTags(ApiTags.Authentication)
            .MapPost(ApiRoutes.Auth.Register, HandleAsync)
            .WithName("RegisterUser")
            .WithSummary("Register a new user account")
            .WithDescription("Creates a new user account with email verification")
            .Produces<ApiResponse>(StatusCodes.Status201Created)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        RegisterUserRequest request,
        ICommandHandler<RegisterUserCommand, RegisterUserVm> handler,
        HttpContext context
    )
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.FirstName,
            request.LastName
        );

        var result = await handler.Handle(command, context.RequestAborted);

        return result.ToCreatedResult(
            $"/{ApiRoutes.Auth.Base}/{ApiRoutes.Auth.Me}",
            ApiMessages.UserRegistered
        );
    }

    /// <summary>
    /// Request model para registro de usuario
    /// </summary>
    public sealed record RegisterUserRequest(
        string Email,
        string Password,
        string ConfirmPassword,
        string FirstName,
        string LastName
    );
}
