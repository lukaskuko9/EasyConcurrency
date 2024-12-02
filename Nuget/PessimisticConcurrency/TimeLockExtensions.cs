using EntityFrameworkCore.PessimisticConcurrency.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.PessimisticConcurrency;

public static class TimeLockExtensions
{
    public static PropertyBuilder<TimeLock?> AddTimeLockConversion(this PropertyBuilder<TimeLock?> propBuilder)
    {
        return propBuilder
            .HasConversion(
                timeLock => timeLock == null ? null : timeLock.Value.Value,
                dateTimeOffset => new TimeLock(dateTimeOffset)
            )
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