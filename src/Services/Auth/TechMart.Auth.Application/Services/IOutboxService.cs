using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Services;

public interface IOutboxService
{
    Task SaveDomainEventsAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default
    );
}
