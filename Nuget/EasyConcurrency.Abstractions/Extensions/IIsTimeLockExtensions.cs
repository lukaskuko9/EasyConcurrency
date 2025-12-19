using EasyConcurrency.Abstractions.IsTimeLock;
using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="TimeLock"/> types
/// </summary>
public static class IsTimeLockExtensions
{
    /// <inheritdoc cref="IIsTimeLock.IsNotLocked()"/>
    public static bool IsNotLocked(this IIsTimeLock? timeLock)
    {
        return timeLock is null || timeLock.IsNotLocked();
    }

    /// <inheritdoc cref="IIsTimeLock.IsNotLocked(DateTimeOffset)"/>
    public static bool IsNotLocked(this IIsTimeLock? timeLock, DateTimeOffset now)
    {
        return timeLock is null || timeLock.IsNotLocked(now);
    }
    
    /// <inheritdoc cref="IIsTimeLock.IsLocked()"/>
    public static bool IsLocked(this IIsTimeLock? timeLock)
    {
        return timeLock is not null && timeLock.IsLocked();
    }
    
    /// <inheritdoc cref="IIsTimeLock.IsLocked(DateTimeOffset)"/>
    public static bool IsLocked(this IIsTimeLock? timeLock, DateTimeOffset now)
    {
        return timeLock is not null && timeLock.IsLocked(now);
    }
}