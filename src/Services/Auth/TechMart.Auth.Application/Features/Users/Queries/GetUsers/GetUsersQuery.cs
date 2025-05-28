using TechMart.Auth.Application.Features.Shared.Queries;
using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Users.Enums;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery : PagedQuery<GetUsersQueryVm>
{
    public UserStatus? Status { get; init; }

    public GetUsersQuery(PaginationBaseQuery pagination, UserStatus? status = null)
    {
        Pagination = pagination;
        Status = status;
    }
}
