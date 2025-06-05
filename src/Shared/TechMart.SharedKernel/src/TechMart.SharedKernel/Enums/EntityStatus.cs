using TechMart.SharedKernel.Base;

namespace TechMart.SharedKernel.Enums;

/// <summary>
/// Represents the status of an entity in the system.
/// </summary>
public enum EntityStatus
{
    /// <summary>
    /// The entity is active and available for use.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The entity is inactive and temporarily unavailable.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// The entity is pending activation or approval.
    /// </summary>
    Pending = 3,

    /// <summary>
    /// The entity is suspended and temporarily blocked.
    /// </summary>
    Suspended = 4,

    /// <summary>
    /// The entity is archived and no longer in regular use.
    /// </summary>
    Archived = 5,

    /// <summary>
    /// The entity is deleted (soft delete).
    /// </summary>
    Deleted = 6
}

/// <summary>
/// Type-safe enumeration for EntityStatus with additional functionality.
/// </summary>
public class EntityStatusEnumeration : Enumeration
{
    public static readonly EntityStatusEnumeration Active = new(1, nameof(Active));
    public static readonly EntityStatusEnumeration Inactive = new(2, nameof(Inactive));
    public static readonly EntityStatusEnumeration Pending = new(3, nameof(Pending));
    public static readonly EntityStatusEnumeration Suspended = new(4, nameof(Suspended));
    public static readonly EntityStatusEnumeration Archived = new(5, nameof(Archived));
    public static readonly EntityStatusEnumeration Deleted = new(6, nameof(Deleted));

    private EntityStatusEnumeration(int id, string name) : base(id, name)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the status represents an available entity.
    /// </summary>
    public bool IsAvailable => this == Active;

    /// <summary>
    /// Gets a value indicating whether the status represents an unavailable entity.
    /// </summary>
    public bool IsUnavailable => this == Inactive || this == Suspended || this == Deleted;

    /// <summary>
    /// Gets a value indicating whether the status represents a temporary state.
    /// </summary>
    public bool IsTemporary => this == Pending || this == Suspended;

    /// <summary>
    /// Gets a value indicating whether the status represents a permanent state.
    /// </summary>
    public bool IsPermanent => this == Active || this == Archived || this == Deleted;

    /// <summary>
    /// Gets all active statuses (available for use).
    /// </summary>
    public static IEnumerable<EntityStatusEnumeration> GetActiveStatuses() =>
        new[] { Active };

    /// <summary>
    /// Gets all visible statuses (not deleted).
    /// </summary>
    public static IEnumerable<EntityStatusEnumeration> GetVisibleStatuses() =>
        new[] { Active, Inactive, Pending, Suspended, Archived };

    /// <summary>
    /// Checks if the status can transition to another status.
    /// </summary>
    /// <param name="targetStatus">The target status to transition to.</param>
    /// <returns>True if the transition is allowed; otherwise, false.</returns>
    public bool CanTransitionTo(EntityStatusEnumeration targetStatus)
    {
        return this switch
        {
            _ when this == Active => targetStatus != Pending,
            _ when this == Inactive => targetStatus != Pending,
            _ when this == Pending => true, // Pending can go to any status
            _ when this == Suspended => targetStatus != Pending,
            _ when this == Archived => targetStatus == Active || targetStatus == Deleted,
            _ when this == Deleted => false, // Deleted entities cannot transition
            _ => false
        };
    }
}