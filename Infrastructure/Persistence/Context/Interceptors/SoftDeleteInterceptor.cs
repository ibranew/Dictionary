using Dictionary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dictionary.Persistence.Context.Interceptors;

//todo buna bakıcam
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null) return result;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is ISoftDeletable softDelete &&
                entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;

                softDelete.IsDeleted = true;
                softDelete.DeletedAt = DateTime.UtcNow;
            }
        }

        return base.SavingChanges(eventData, result);
    }
}
