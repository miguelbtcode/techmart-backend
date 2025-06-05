using TechMart.SharedKernel.Abstractions;

namespace TechMart.SharedKernel.Base;

/// <summary>
/// Base class for auditable entities with Guid identifiers.
/// </summary>
public abstract class BaseAuditableEntity : BaseAuditableEntity<Guid>
{
    protected BaseAuditableEntity() : base()
    {
    }

    protected BaseAuditableEntity(Guid id) : base(id)
    {
    }
}

/// <summary>
/// Base class for auditable entities with custom identifier types.
/// Extends BaseEntity with additional audit fields for tracking who created/updated the entity.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity<TId>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    protected BaseAuditableEntity() : base()
    {
    }

    protected BaseAuditableEntity(TId id) : base(id)
    {
    }

    /// <summary>
    /// Sets the audit information when the entity is created.
    /// </summary>
    /// <param name="userId">The identifier of the user creating the entity.</param>
    protected void SetCreatedBy(string userId)
    {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the audit information when the entity is updated.
    /// </summary>
    /// <param name="userId">The identifier of the user updating the entity.</param>
    protected void SetUpdatedBy(string userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the audit information when the entity is updated with a specific date.
    /// </summary>
    /// <param name="userId">The identifier of the user updating the entity.</param>
    /// <param name="updatedAt">The date and time when the entity was updated.</param>
    protected void SetUpdatedBy(string userId, DateTime updatedAt)
    {
        UpdatedBy = userId;
        UpdatedAt = updatedAt;
    }
}