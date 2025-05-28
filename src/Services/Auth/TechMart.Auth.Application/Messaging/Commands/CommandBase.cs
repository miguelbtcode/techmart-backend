namespace TechMart.Auth.Application.Messaging.Commands;

/// <summary>
/// Base class for commands with common properties
/// </summary>
public abstract record CommandBase : ICommand
{
    /// <summary>
    /// Unique identifier for this command instance
    /// </summary>
    public Guid CommandId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// When this command was created
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// User who initiated this command (if applicable)
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Optional correlation ID for tracking related operations
    /// </summary>
    public string? CorrelationId { get; init; }
}

/// <summary>
/// Base class for commands with response and common properties
/// </summary>
public abstract record CommandBase<TResponse> : ICommand<TResponse>
{
    /// <summary>
    /// Unique identifier for this command instance
    /// </summary>
    public Guid CommandId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// When this command was created
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// User who initiated this command (if applicable)
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Optional correlation ID for tracking related operations
    /// </summary>
    public string? CorrelationId { get; init; }
}
