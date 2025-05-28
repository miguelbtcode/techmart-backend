using TechMart.Auth.Application.Features.Shared.Vms;
using TechMart.Auth.Application.Features.Users.Specifications;
using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUsers;

internal sealed class GetUsersQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUsersQuery, PaginationVm<GetUsersQueryVm>>
{
    public async Task<Result<PaginationVm<GetUsersQueryVm>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        var spec = new UserPagedSpec(request.Pagination, request.Status?.ToString());

        var users = await unitOfWork.Users.ListAsync(spec, cancellationToken);

        var totalCount = await unitOfWork.Users.CountAsync(spec, cancellationToken);

        var userIds = users.Select(u => u.Id).ToList();

        var roles = await unitOfWork.UserRoles.FindAsync(
            r => userIds.Contains(r.UserId),
            cancellationToken
        );

        var data = users
            .Select(user =>
            {
                var userRoles = roles
                    .Where(r => r.UserId == user.Id)
                    .Select(r => r.Role?.Name ?? "Unknown")
                    .ToList();

                return new GetUsersQueryVm(
                    user.Id.Value,
                    user.Email.Value,
                    user.FirstName,
                    user.LastName,
                    user.GetFullName(),
                    user.Status.ToString(),
                    user.IsEmailConfirmed,
                    user.LastLoginAt,
                    user.CreatedAt,
                    userRoles
                );
            })
            .ToList();

        var paginationVm = new PaginationVm<GetUsersQueryVm>
        {
            Data = data,
            Count = totalCount,
            PageIndex = request.Pagination.PageIndex,
            PageSize = request.Pagination.PageSize,
            PageCount = (int)Math.Ceiling((double)totalCount / request.Pagination.PageSize),
            ResultByPage = data.Count,
        };

        return Result.Success(paginationVm);
    }
}
