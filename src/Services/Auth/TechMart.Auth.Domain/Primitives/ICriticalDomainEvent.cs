namespace TechMart.Auth.Domain.Primitives;

/// <summary>
/// Marca eventos que deben procesarse inmediatamente (críticos para UX)
/// </summary>
public interface ICriticalDomainEvent : IDomainEvent
{
    /// <summary>
    /// Prioridad del evento (1 = más crítico)
    /// </summary>
    int Priority => 1;

    /// <summary>
    /// Maximum retry attempts for critical events
    /// </summary>
    int MaxRetries => 3;
}
