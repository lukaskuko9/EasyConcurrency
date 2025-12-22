using EasyConcurrency.Abstractions.TimeLock;
using EasyConcurrency.EntityFramework.ValueConverter;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyConcurrency.EntityFramework.Extensions;

/// <summary>
/// Extension methods for <see cref="Abstractions.TimeLock.TimeLock"/> types
/// </summary>
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Configures the <see cref="TimeLock"/> property so that the property value is converted to <see cref="DateTimeOffset"/>
    /// before writing to the database and converted back when reading from the database.
    /// </summary>
    /// <param name="propBuilder">Property builder of <see cref="TimeLock"/></param>
    /// <code>
    /// modelBuilder.Entity&lt;MyTimeLockEntity&gt;(entityBuilder =>
    ///{
    ///...
    ///entityBuilder.Property(refundEntity => refundEntity.LockedUntil).AddTimeLockConversion();
    ///...
    ///}
    /// </code>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<TimeLock?> AddTimeLockConversion(this PropertyBuilder<TimeLock?> propBuilder)
    {
        return propBuilder
            .HasConversion<TimeLockConverter>()
            .IsConcurrencyToken()
            .IsRequired(false);
    }
}