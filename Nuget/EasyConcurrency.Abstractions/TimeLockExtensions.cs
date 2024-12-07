namespace EasyConcurrency.Abstractions;

public static class TimeLockExtensions
{
    public static bool IsNotLocked(this TimeLock? timeLock)
    {
        return timeLock is null || timeLock.Value.IsNotLocked();
    }
    
    public static bool IsNotLocked(this TimeLock? timeLock, DateTimeOffset now)
    {
        return timeLock is null || timeLock.Value.IsNotLocked(now);
    }
}