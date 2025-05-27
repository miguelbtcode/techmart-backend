using Microsoft.AspNetCore.Authorization;
using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Shared.Queries;
using TechMart.Auth.Application.Features.Shared.Vms;
using TechMart.Auth.Application.Features.Users.Queries.GetAllUsers;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Users.Enums;

namespace TechMart.Auth.API.Endpoints.Users;

/// <summary>
/// Endpoint para obtener lista de usuarios
/// </summary>
internal sealed class GetAllUsersEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Users.Base)
            .WithTags(ApiTags.Users)
            .MapGet("", HandleAsync)
            .WithName("GetAllUsers")
            .WithSummary("Get paginated list of users")
            .WithDescription("Returns a paginated list of users with optional filtering")
            .RequireAuthorization()
            .Produces<PaginatedResponse<UserListVm>>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse>(StatusCodes.Status403Forbidden);
    }

    [Authorize(Roles = "Administrator,Moderator")]
    private static async Task<IResult> HandleAsync(
        [AsParameters] GetUsersRequest request,
        IQueryHandler<GetAllUsersQuery, PaginationVm<UserListVm>> handler,
        HttpContext context
    )
    {
        var pagination = new PaginationBaseQuery
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            SearchTerm = request.SearchTerm,
            SortBy = request.SortBy,
        };

        var query = new GetAllUsersQuery(pagination, request.Status);
        var result = await handler.Handle(query, context.RequestAborted);

        return result.ToPaginatedHttpResult(ApiMessages.UsersRetrieved);
    }

    /// <summary>
    /// Request parameters para obtener usuarios
    /// </summary>
    public sealed record GetUsersRequest(
        int PageIndex = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        string? SortBy = null,
        UserStatus? Status = null
    );
}
