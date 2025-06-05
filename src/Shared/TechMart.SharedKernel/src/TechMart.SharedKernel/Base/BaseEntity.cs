using TechMart.SharedKernel.Abstractions;

namespace TechMart.SharedKernel.Base;

/// <summary>
/// Base class for entities with Guid identifiers.
/// </summary>
public abstract class BaseEntity : BaseEntity<Guid>
{
    protected BaseEntity() : base()
    {
    }

    protected BaseEntity(Guid id) : base(id)
    {
    }
}

/// <summary>
/// Base class for entities with custom identifier types.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class BaseEntity<TId> : IEntity<TId>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public virtual TId Id { get; protected set; }

    protected BaseEntity()
    {
    }

    protected BaseEntity(TId id) : this()
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id?.Equals(default(TId)) == true || other.Id?.Equals(default(TId)) == true)
            return false;

        return Id?.Equals(other.Id) == true;
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public static bool operator ==(BaseEntity<TId>? a, BaseEntity<TId>? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(BaseEntity<TId>? a, BaseEntity<TId>? b)
    {
        return !(a == b);
    }
}