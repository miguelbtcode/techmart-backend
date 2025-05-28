using TechMart.Auth.Application.Features.Shared.Queries;
using TechMart.Auth.Application.Features.Shared.Vms;

namespace TechMart.Auth.Application.Messaging.Queries;

public abstract record PagedQuery<TResponse> : IQuery<PaginationVm<TResponse>>
    where TResponse : class
{
    public PaginationBaseQuery Pagination { get; init; }
}
