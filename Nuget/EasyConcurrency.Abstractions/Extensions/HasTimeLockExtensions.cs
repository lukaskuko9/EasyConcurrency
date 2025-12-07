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
    /// <param name="entity">Entity instance with<see cref="TimeLock"/> to check</param>
    /// <param name="now">Specifies the current time to be used when comparing if the entity is locked or not.</param>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public static bool IsNotLocked(this IHasTimeLock entity, DateTimeOffset now)
    {
        return entity.LockedUntil.IsNotLocked(now);
    }
        
    /// <summary>
    /// Sets the <see cref="TimeLock"/> on the entity. This entity will be locked for <paramref name="lockTimeDuration"/> duration.
    /// </summary>
    /// <param name="entity">Entity instance with<see cref="TimeLock"/> to set the lock for</param>
    /// <param name="lockTimeDuration">How long to lock the entity for when using <see cref="DateTimeOffset"/> as current date and time.</param>
    /// <remarks>This does not persist changes in data source where the entity should be locked.
    /// Request to save changes needs to be sent to the data source for this lock to take effect.</remarks>
    /// <returns>True, if this entity was successfully locked,
    /// false if entity cannot be locked now, as there is already lock present on this entity.</returns>
    public static bool SetLock(this IHasTimeLock entity, TimeLock.TimeLock lockTimeDuration)
    {
        if (entity.LockedUntil != null) 
            return entity.LockedUntil.Value.SetLock(lockTimeDuration);
        
        entity.LockedUntil = TimeLock.TimeLock.Create(lockTimeDuration);
        return true;
    }
    
    /// <summary>
    /// Sets the <see cref="TimeLock"/> on the entity. This entity will be locked for specified amount of <paramref name="lockTimeDuration"/>.
    /// </summary>
    /// <param name="entity">Entity instance with<see cref="TimeLock"/> to set the lock for</param>
    /// <param name="lockTimeDuration">How long to lock the entity for when using <see cref="DateTimeOffset.UtcNow"/> as current date and time.</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws this exception if the <paramref name="lockTimeDuration"/> argument is negative.</exception>
    /// <remarks>This does not persist changes in data source where the entity should be locked.
    /// Request to save changes needs to be sent to the data source for this lock to take effect.</remarks>
    /// <returns>True, if this entity was successfully locked,
    /// false if entity cannot be locked now, as there is already lock present on this entity.</returns>
    public static bool SetLock(this IHasTimeLock entity, TimeSpan lockTimeDuration)
    {
        if (entity.LockedUntil != null) 
            return entity.LockedUntil.Value.SetLock(lockTimeDuration);
        
        entity.LockedUntil = TimeLock.TimeLock.Create(DateTimeOffset.UtcNow.Add(lockTimeDuration));
        return true;
    }
        
    /// <summary>
    /// Sets the <see cref="TimeLock"/> on the entity. This entity will be locked for specified amount of <paramref name="minutes"/>.
    /// </summary>
    /// <param name="entity">Entity instance with<see cref="TimeLock"/> to set the lock for</param>
    /// <param name="minutes">How long to lock the entity for in minutes when using <see cref="DateTimeOffset.UtcNow"/> as current date and time.</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws this exception if the <paramref name="minutes"/> argument is negative.</exception>
    /// <remarks>This does not persist changes in data source where the entity should be locked.
    /// Request to save changes needs to be sent to the data source for this lock to take effect.</remarks>
    /// <returns>True, if this entity was successfully locked,
    /// false if entity cannot be locked now, as there is already lock present on this entity.</returns>

    public static bool SetLock(this IHasTimeLock entity, int minutes)
    {
        if (entity.LockedUntil != null)
            return entity.LockedUntil.Value.SetLock(minutes);
        
        entity.LockedUntil = TimeLock.TimeLock.Create(DateTimeOffset.UtcNow.AddMinutes(minutes));
        return true;
    }
    
    /// <summary>
    /// Unlocks the <see cref="TimeLock"/> on the entity by setting it to null value.
    /// Useful for when the entity was claimed by current process which has finished operating on it.
    /// Calling this method and persisting changes will make it available for other processes.
    /// </summary>
    /// <remarks>This does not persist changes in data source where the entity should be locked.
    /// Request to save changes needs to be sent to the data source for this lock to take effect.</remarks>
    public static void Unlock(this IHasTimeLock entity)
    { 
        entity.LockedUntil = null;
    }
}