using EasyConcurrency.Abstractions.HasTimeLock;
using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.Extensions;

/// <summary>
/// 
/// </summary>
public static class HasTimeLockExtensions
{
    
    /// <summary>
    /// Checks whether this entity is not locked.
    /// </summary>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public static bool IsNotLocked(this IHasTimeLock entity)
    {
        return entity.LockedUntil.IsNotLocked();
    }
        
    /// <summary>
    /// Checks whether entity using this TimeLock is not locked.
    /// </summary>
    /// <param name="timeLock"><see cref="TimeLock"/> instance to check</param>
    /// <param name="now">Specifies the current time to be used when comparing if the entity is locked or not.</param>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public static bool IsNotLocked(this IHasTimeLock entity, DateTimeOffset now)
    {
        return entity.LockedUntil.IsNotLocked(now);
    }
        
    /// <inheritdoc />
    public static bool SetLock(this IHasTimeLock entity, TimeLock.TimeLock timeLock)
    {
        if (entity.LockedUntil != null) 
            return entity.LockedUntil.Value.SetLock(timeLock);
        
        entity.LockedUntil = TimeLock.TimeLock.Create(timeLock);
        return true;
    }
    
    /// <inheritdoc />
    public static bool SetLock(this IHasTimeLock entity, TimeSpan lockTimeDuration)
    {
        if (entity.LockedUntil != null) 
            return entity.LockedUntil.Value.SetLock(lockTimeDuration);
        
        entity.LockedUntil = TimeLock.TimeLock.Create(DateTimeOffset.UtcNow.Add(lockTimeDuration));
        return true;
    }
        
    /// <inheritdoc />
    public static bool SetLock(this IHasTimeLock entity, int minutes)
    {
        if (entity.LockedUntil != null)
            return entity.LockedUntil.Value.SetLock(minutes);
        
        entity.LockedUntil = TimeLock.TimeLock.Create(DateTimeOffset.UtcNow.AddMinutes(minutes));
        return true;
    }
    
    /// <inheritdoc />
    public static void Unlock(this IHasTimeLock entity)
    { 
        entity.LockedUntil = null;
    }
}