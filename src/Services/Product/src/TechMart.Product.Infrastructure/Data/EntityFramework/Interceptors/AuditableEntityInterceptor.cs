using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TechMart.Product.Application.Contracts.Identity;
using TechMart.SharedKernel.Abstractions;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Infrastructure.Data.EntityFramework.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public AuditableEntityInterceptor(
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var currentUserId = _currentUserService.UserId;
        var currentTime = _dateTimeService.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.CreatedAt = currentTime;
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Entity.UpdatedAt = currentTime;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Entity.UpdatedAt = currentTime;
                    break;
            }
        }
    }
}