using TechMart.Auth.Application.Features.Shared.Queries;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.Entities;

namespace TechMart.Auth.Application.Features.Users.Specifications;

public sealed class UserPagedSpec : Specification<User>
{
    public UserPagedSpec(PaginationBaseQuery request, string? statusFilter)
    {
        // Filtro (opcional)
        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            SetCriteria(u => u.Status.ToString() == statusFilter);
        }

        // Búsqueda
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var lowerTerm = request.SearchTerm.ToLower();
            SetCriteria(u =>
                u.Email.Value.ToLower().Contains(lowerTerm)
                || u.FirstName.ToLower().Contains(lowerTerm)
                || u.LastName.ToLower().Contains(lowerTerm)
            );
        }

        // Ordenamiento
        switch (request.SortBy?.ToLower())
        {
            case "email":
                ApplyOrderBy(u => u.Email.Value);
                break;
            case "firstname":
                ApplyOrderBy(u => u.FirstName);
                break;
            case "lastname":
                ApplyOrderBy(u => u.LastName);
                break;
            case "lastloginat":
                ApplyOrderByDescending(u => u.LastLoginAt ?? DateTime.MinValue);
                break;
            default:
                ApplyOrderByDescending(u => u.CreatedAt);
                break;
        }

        // Paginación
        ApplyPaging((request.PageIndex - 1) * request.PageSize, request.PageSize);
    }
}
