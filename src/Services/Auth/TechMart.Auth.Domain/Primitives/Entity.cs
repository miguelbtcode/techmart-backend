namespace TechMart.Auth.Domain.Primitives;

/// <summary>
/// Base class for all entities with audit properties
/// </summary>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public TId Id { get; protected set; }

    /// <summary>
    /// User ID who created this entity
    /// </summary>
    public Guid? CreatedBy { get; protected set; }

    /// <summary>
    /// User ID who last updated this entity
    /// </summary>
    public Guid? UpdatedBy { get; protected set; }

    /// <summary>
    /// When this entity was created
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// When this entity was last updated
    /// </summary>
    public DateTime UpdatedAt { get; protected set; }

    /// <summary>
    /// Protected constructor for derived classes
    /// </summary>
    protected Entity(TId id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        Id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Parameterless constructor for EF Core
    /// </summary>
    protected Entity()
    {
        Id = default!;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the audit information when entity is modified
    /// </summary>
    /// <param name="updatedBy">User ID who is updating the entity</param>
    protected void UpdateAudit(Guid? updatedBy = null)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the creator information (typically used during entity creation)
    /// </summary>
    /// <param name="createdBy">User ID who created the entity</param>
    protected void SetCreatedBy(Guid createdBy)
    {
        CreatedBy = createdBy;
        // Also set UpdatedBy to the same user initially
        UpdatedBy = createdBy;
    }

    #region Equality Implementation

    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        if (GetType() != other.GetType())
            return false;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity<TId>);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    #endregion

    public override string ToString()
    {
        return $"{GetType().Name} [Id: {Id}]";
    }
}
