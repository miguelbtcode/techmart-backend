using TechMart.SharedKernel.Common;

namespace TechMart.SharedKernel.Exceptions;

/// <summary>
/// Exception thrown when a conflict occurs, such as attempting to create a duplicate entity.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Gets the type of entity that caused the conflict.
    /// </summary>
    public string? EntityType { get; }

    /// <summary>
    /// Gets the identifier of the entity that caused the conflict.
    /// </summary>
    public object? EntityId { get; }

    /// <summary>
    /// Gets the property that caused the conflict.
    /// </summary>
    public string? ConflictProperty { get; }

    /// <summary>
    /// Gets the value that caused the conflict.
    /// </summary>
    public object? ConflictValue { get; }

    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ConflictException(string entityType, object entityId, string message) : base(message)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public ConflictException(string entityType, string conflictProperty, object conflictValue, string message) : base(message)
    {
        EntityType = entityType;
        ConflictProperty = conflictProperty;
        ConflictValue = conflictValue;
    }

    /// <summary>
    /// Creates a ConflictException for a duplicate entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns>A ConflictException.</returns>
    public static ConflictException ForDuplicate<T>(object entityId) =>
        new(typeof(T).Name, entityId, $"{typeof(T).Name} with identifier '{entityId}' already exists.");

    /// <summary>
    /// Creates a ConflictException for a duplicate property value.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="propertyName">The property name.</param>
    /// <param name="propertyValue">The property value.</param>
    /// <returns>A ConflictException.</returns>
    public static ConflictException ForDuplicateProperty<T>(string propertyName, object propertyValue) =>
        new(typeof(T).Name, propertyName, propertyValue, 
            $"{typeof(T).Name} with {propertyName} '{propertyValue}' already exists.");

    /// <summary>
    /// Creates a ConflictException for a business rule violation.
    /// </summary>
    /// <param name="rule">The business rule that was violated.</param>
    /// <returns>A ConflictException.</returns>
    public static ConflictException ForBusinessRule(string rule) =>
        new($"Business rule violation: {rule}");

    /// <summary>
    /// Converts the exception to an Error.
    /// </summary>
    /// <returns>An Error representing the conflict.</returns>
    public Error ToError()
    {
        var code = EntityType != null ? $"{EntityType}.Conflict" : "Conflict";
        return Error.Conflict(code, Message);
    }
}