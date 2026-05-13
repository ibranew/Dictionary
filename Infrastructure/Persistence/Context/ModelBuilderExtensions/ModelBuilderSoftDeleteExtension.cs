using Dictionary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Dictionary.Persistence.Context.ModelBuilderExtensions;

public static class ModelBuilderSoftDeleteExtension
{
    public static void ApplySoftDeleteFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ModelBuilderSoftDeleteExtension)
                    .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { modelBuilder });
            }
        }
    }
    private static void GetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(x => !x.IsDeleted);
    }
}
