using Microsoft.AspNetCore.Authorization;
using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Features.Users.Queries.GetUserById;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.API.Endpoints.Users;

/// <summary>
/// Endpoint para obtener usuario por ID
/// </summary>
internal sealed class GetUserByIdEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Users.Base)
            .WithTags(ApiTags.Users)
            .MapGet("{userId:guid}", HandleAsync)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("Returns detailed information about a specific user")
            .RequireAuthorization()
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse>(StatusCodes.Status403Forbidden);
    }

    [Authorize(Roles = "Administrator,Moderator")]
    private static async Task<IResult> HandleAsync(
        Guid userId,
        IQueryHandler<GetUserByIdQuery, UserDetailVm> handler,
        HttpContext context
    )
    {
        var query = new GetUserByIdQuery(userId);
        var result = await handler.Handle(query, context.RequestAborted);

        return result.ToHttpResult(ApiMessages.UserDetailsRetrieved);
    }
}
