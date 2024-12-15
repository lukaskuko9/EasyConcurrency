namespace EasyConcurrency.Abstractions.TimeLock;

public static class TimeLockExtensions
{
    /// <summary>
    /// Checks whether this entity is not locked.
    /// </summary>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public static bool IsNotLocked(this TimeLock? timeLock)
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
    public static bool IsNotLocked(this TimeLock? timeLock, DateTimeOffset now)
    {
        return timeLock is null || timeLock.Value.IsNotLocked(now);
    }
}