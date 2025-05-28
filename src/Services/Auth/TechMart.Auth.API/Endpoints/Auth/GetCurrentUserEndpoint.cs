using Microsoft.AspNetCore.Authorization;
using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Features.Users.Queries.GetCurrentUser;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.API.Endpoints.Auth;

/// <summary>
/// Endpoint para obtener información del usuario actual
/// </summary>
internal sealed class GetCurrentUserEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Auth.Base)
            .WithTags(ApiTags.Authentication)
            .MapGet(ApiRoutes.Auth.Me, HandleAsync)
            .WithName("GetCurrentUser")
            .WithSummary("Get current authenticated user")
            .WithDescription("Returns information about the currently authenticated user")
            .RequireAuthorization()
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized);
    }

    [Authorize]
    private static async Task<IResult> HandleAsync(
        HttpContext context,
        IQueryHandler<GetCurrentUserQuery, UserDetailVm> handler
    )
    {
        var authHeader = context.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Results.Json(
                ApiResponse.Failure(ApiMessages.InvalidCredentials),
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        var accessToken = authHeader["Bearer ".Length..].Trim();
        var query = new GetCurrentUserQuery(accessToken);
        var result = await handler.Handle(query, context.RequestAborted);

        return result.ToHttpResult(ApiMessages.UserDetailsRetrieved);
    }
}
