﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyConcurrency.EntityFramework.TimeLock;

/// <summary>
/// Extension methods for <see cref="EasyConcurrency.Abstractions.TimeLockNamespace.TimeLock"/> types
/// </summary>
public static class TimeLockExtensions
{
    /// <summary>
    /// Configures the <see cref="TimeLock"/> property so that the property value is converted to <see cref="DateTimeOffset"/>
    /// before writing to the database and converted back when reading from the database.
    /// </summary>
    /// <param name="propBuilder">Property builder of <see cref="TimeLock"/></param>
    /// <code>
    /// modelBuilder.Entity&lt;MyLockableEntity&gt;(entityBuilder =>
    ///{
    ///...
    ///entityBuilder.Property(refundEntity => refundEntity.LockedUntil).AddTimeLockConversion();
    ///...
    ///}
    /// </code>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<Abstractions.TimeLockNamespace.TimeLock?> AddTimeLockConversion(this PropertyBuilder<Abstractions.TimeLockNamespace.TimeLock?> propBuilder)
    {
        return propBuilder
            .HasConversion<TimeLockConverter>()
            .IsRequired(false);
    }
}