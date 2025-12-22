using EasyConcurrency.Abstractions.PessimisticLock;
using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="IPessimisticLock"/> interfaces
/// </summary>
public static class PessimisticLockExtensions
{
    /// <inheritdoc cref="IPessimisticLock.IsNotLocked()"/>
    public static bool IsNotLocked(this IPessimisticLock? timeLock)
    {
        return timeLock is null || timeLock.IsNotLocked();
    }
    
    /// <inheritdoc cref="IPessimisticLock.IsLocked()"/>
    public static bool IsLocked(this IPessimisticLock? timeLock)
    {
        return timeLock is not null && timeLock.IsLocked();
    }

}