using TechMart.Auth.Application.Features.Shared.Vms;
using TechMart.Auth.Application.Features.Users.Specifications;
using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Users.Queries.GetAllUsers;

internal sealed class GetAllUsersQueryHandler
    : IQueryHandler<GetAllUsersQuery, PaginationVm<UserListVm>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginationVm<UserListVm>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        var spec = new UserPagedSpec(request.Pagination, request.Status?.ToString());

        var users = await _unitOfWork.Users.ListAsync(spec, cancellationToken);

        var totalCount = await _unitOfWork.Users.CountAsync(spec, cancellationToken);

        var userIds = users.Select(u => u.Id).ToList();

        var roles = await _unitOfWork.UserRoles.FindAsync(
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

                return new UserListVm(
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

        var paginationVm = new PaginationVm<UserListVm>
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
