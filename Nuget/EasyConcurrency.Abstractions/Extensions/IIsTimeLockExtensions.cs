using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="ITimeLock"/> interfaces
/// </summary>
public static class TimeLockExtensions
{
    /// <inheritdoc cref="ITimeLock.IsNotLocked(DateTimeOffset)"/>
    public static bool IsNotLocked(this ITimeLock? timeLock, DateTimeOffset now)
    {
        return timeLock is null || timeLock.IsNotLocked(now);
    }

    /// <inheritdoc cref="ITimeLock.IsLocked(DateTimeOffset)"/>
    public static bool IsLocked(this ITimeLock? timeLock, DateTimeOffset now)
    {
        return timeLock is not null && timeLock.IsLocked(now);
    }
}