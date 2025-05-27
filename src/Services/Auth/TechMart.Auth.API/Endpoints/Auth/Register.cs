using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TechMart.Auth.API.Controllers;
using TechMart.Auth.API.Controllers.Base;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Commands.RegisterUser;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.API.Endpoints.Auth;

internal sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication");

        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .WithSummary("Register a new user account")
            .WithDescription("Creates a new user account with the provided information")
            .Produces<ApiResponse<RegisterUserVm>>(StatusCodes.Status201Created)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .AddEndpointFilter<ValidationFilter<RegisterUserRequest>>();
    }

    private static async Task<Results<Created<ApiResponse<RegisterUserVm>>, BadRequest<ApiResponse>>> RegisterAsync(
        [FromBody] RegisterUserRequest request,
        ICommandHandler<RegisterUserCommand, RegisterUserVm> handler,
        HttpContext httpContext)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.FirstName,
            request.LastName);

        var result = await handler.Handle(command, httpContext.RequestAborted);

        return result.IsSuccess
            ? TypedResults.Created("/api/auth/me", ApiResponse<RegisterUserVm>.Success(result.Value, "User registered successfully"))
            : TypedResults.BadRequest(ApiResponse.Error(result.Error));
    }
}
}
