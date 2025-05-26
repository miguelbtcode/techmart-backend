using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Features.Shared.Queries;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Domain.Users.Enums;

namespace TechMart.Auth.Application.Features.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : PagedQuery<UserListVm>
{
    public UserStatus? Status { get; init; }

    public GetAllUsersQuery(PaginationBaseQuery pagination, UserStatus? status = null)
    {
        Pagination = pagination;
        Status = status;
    }
}
