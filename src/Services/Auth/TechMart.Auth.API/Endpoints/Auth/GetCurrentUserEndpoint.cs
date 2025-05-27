using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Extensions;
using TechMart.Auth.API.Common;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Users.Queries.GetCurrentUser;
using TechMart.Auth.Application.Features.Users.Vms;

namespace TechMart.Auth.API.Endpoints.Auth;

internal sealed class GetCurrentUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication");

        group
            .MapGet("/me", GetCurrentUserAsync)
            .WithName("GetCurrentUser")
            .WithSummary("Get current authenticated user information")
            .WithDescription("Returns information about the currently authenticated user")
            .RequireAuthorization()
            .Produces<ApiResponse<UserDetailVm>>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized);
    }

    private static async Task<
        Results<Ok<ApiResponse<UserDetailVm>>, Unauthorized<ApiResponse>>
    > GetCurrentUserAsync(
        HttpContext httpContext,
        IQueryHandler<GetCurrentUserQuery, UserDetailVm> handler
    )
    {
        var authHeader = httpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return TypedResults.Unauthorized();
        }

        var accessToken = authHeader["Bearer ".Length..].Trim();
        var query = new GetCurrentUserQuery(accessToken);
        var result = await handler.Handle(query, httpContext.RequestAborted);

        if (result.IsSuccess)
        {
            var successResponse = ApiResponse<UserDetailVm>.Successfull(result.Value);
            return TypedResults.Ok(successResponse);
        }

        return TypedResults.Unauthorized();
    }
}
