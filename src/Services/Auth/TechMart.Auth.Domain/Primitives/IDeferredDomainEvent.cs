namespace TechMart.Auth.Domain.Primitives;

/// <summary>
/// Marca eventos que pueden procesarse en background
/// </summary>
public interface IDeferredDomainEvent : IDomainEvent
{
    /// <summary>
    /// Delay antes de procesar (opcional)
    /// </summary>
    TimeSpan? ProcessingDelay => null;

    /// <summary>
    /// Priority for background processing (1 = highest priority)
    /// </summary>
    int BackgroundPriority => 5;
}
