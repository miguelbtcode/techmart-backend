using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Messaging.Queries;

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
