using EasyConcurrency.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyConcurrency.EntityFramework;

public static class TimeLockExtensions
{
    public static PropertyBuilder<TimeLock?> AddTimeLockConversion(this PropertyBuilder<TimeLock?> propBuilder)
    {
        return propBuilder
            .HasConversion<TimeLockConverter>()
            .IsRequired(false);
    }
    
    public static PropertyBuilder<TimeLock> AddTimeLockConversion(this PropertyBuilder<TimeLock> propBuilder)
    {
        return propBuilder
            .HasConversion(
                timeLock => timeLock.Value,
                dateTimeOffset => new TimeLock(dateTimeOffset)
            )
            .IsRequired();
    }
}