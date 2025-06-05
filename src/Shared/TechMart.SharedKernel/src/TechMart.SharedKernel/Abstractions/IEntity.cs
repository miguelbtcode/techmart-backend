namespace TechMart.SharedKernel.Abstractions;

/// <summary>
/// Marker interface for entities in Domain-Driven Design.
/// An entity is an object that is not defined by its attributes, but rather by a thread of continuity and its identity.
/// </summary>
public interface IEntity<TId>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    TId Id { get; }
}