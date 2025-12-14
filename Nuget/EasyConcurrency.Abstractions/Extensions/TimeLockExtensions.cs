using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="TimeLock"/> types
/// </summary>
public static class TimeLockExtensions
{
    /// <summary>
    /// Checks whether this entity is not locked.
    /// </summary>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public static bool IsNotLocked(this TimeLock.TimeLock? timeLock)
    {
        return timeLock is null || timeLock.Value.IsNotLocked();
    }

    /// <summary>
    /// Checks whether entity using this TimeLock is not locked.
    /// </summary>
    /// <param name="timeLock"><see cref="TimeLock"/> instance to check</param>
    /// <param name="now">Specifies the current time to be used when comparing if the entity is locked or not.</param>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public static bool IsNotLocked(this TimeLock.TimeLock? timeLock, DateTimeOffset now)
    {
        return timeLock is null || timeLock.Value.IsNotLocked(now);
    }
    
    /// <summary>
    /// Checks whether this entity is locked at specified time.
    /// </summary>
    /// <param name="timeLock"><see cref="TimeLock"/> instance to check</param>
    /// <returns>Returns true if entity is locked and therefore already claimed, otherwise false.</returns>
    public static bool IsLocked(this TimeLock.TimeLock? timeLock)
    {
        return timeLock is not null && timeLock.Value.IsLocked();
    }

    /// <summary>
    /// Checks whether this entity is locked at specified time.
    /// </summary>
    /// <param name="timeLock"><see cref="TimeLock"/> instance to check</param>
    /// <param name="now">Specifies the current time to be used when comparing if the entity is locked or not.</param>
    /// <returns>Returns true if entity is locked and therefore already claimed, otherwise false.</returns>
    public static bool IsLocked(this TimeLock.TimeLock? timeLock, DateTimeOffset now)
    {
        return timeLock is not null &&  timeLock.Value.IsLocked(now);
    }
}