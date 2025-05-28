using TechMart.Auth.API.Common.Constants;
using TechMart.Auth.API.Common.Responses;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application.Features.Users.Queries.CheckEmailAvailability;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.API.Endpoints.Users;

/// <summary>
/// Endpoint para verificar disponibilidad de email
/// </summary>
internal sealed class CheckEmailAvailabilityEndpoint : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup(ApiRoutes.Users.Base)
            .WithTags(ApiTags.Users)
            .MapGet("check-email", HandleAsync)
            .WithName("CheckEmailAvailability")
            .WithSummary("Check email availability")
            .WithDescription("Checks if an email address is available for registration")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] CheckEmailRequest request,
        IQueryHandler<CheckEmailAvailabilityQuery, EmailAvailabilityVm> handler,
        HttpContext context
    )
    {
        var query = new CheckEmailAvailabilityQuery(request.Email, request.ExcludeUserId);
        var result = await handler.Handle(query, context.RequestAborted);

        return result.ToHttpResult("Email availability checked successfully");
    }

    /// <summary>
    /// Request parameters para verificar email
    /// </summary>
    public sealed record CheckEmailRequest(string Email, Guid? ExcludeUserId = null);
}
