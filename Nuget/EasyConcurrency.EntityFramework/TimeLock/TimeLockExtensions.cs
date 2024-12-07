using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyConcurrency.EntityFramework.TimeLock;

public static class TimeLockExtensions
{
    public static PropertyBuilder<Abstractions.TimeLock?> AddTimeLockConversion(this PropertyBuilder<Abstractions.TimeLock?> propBuilder)
    {
        return propBuilder
            .HasConversion<TimeLockConverter>()
            .IsRequired(false);
    }
}