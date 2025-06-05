using TechMart.SharedKernel.Common;

namespace TechMart.SharedKernel.Exceptions;

/// <summary>
/// Exception thrown when a requested entity or resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Gets the type of entity that was not found.
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the identifier of the entity that was not found.
    /// </summary>
    public object EntityId { get; }

    public NotFoundException(string entityType, object entityId) 
        : base($"{entityType} with identifier '{entityId}' was not found.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public NotFoundException(string entityType, object entityId, string message) 
        : base(message)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public NotFoundException(string entityType, object entityId, string message, Exception innerException) 
        : base(message, innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Creates a NotFoundException for a specific entity type and identifier.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns>A NotFoundException.</returns>
    public static NotFoundException For<T>(object entityId) =>
        new(typeof(T).Name, entityId);

    /// <summary>
    /// Creates a NotFoundException with a custom message.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="message">The custom message.</param>
    /// <returns>A NotFoundException.</returns>
    public static NotFoundException For<T>(object entityId, string message) =>
        new(typeof(T).Name, entityId, message);

    /// <summary>
    /// Converts the exception to an Error.
    /// </summary>
    /// <returns>An Error representing the not found condition.</returns>
    public Error ToError() =>
        Error.NotFound($"{EntityType}.NotFound", Message);
}