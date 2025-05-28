using TechMart.Auth.Application.Messaging.Queries;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Features.Roles.Queries.GetRoles;

internal sealed class GetRolesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetRolesQuery, IEnumerable<GetRolesQueryVm>>
{
    public async Task<Result<IEnumerable<GetRolesQueryVm>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        var roles = await unitOfWork.Roles.GetAllAsync(cancellationToken);

        var roleDtos = roles.Select(r => new GetRolesQueryVm(
            r.Id.Value,
            r.Name,
            r.Description,
            r.HierarchyLevel,
            r.CreatedAt
        ));

        return Result.Success(roleDtos);
    }
}
