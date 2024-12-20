namespace EasyConcurrency.Abstractions.TimeLockNamespace;

/// <summary>
/// Provides basic interface for determining, if an instance is locked or not.
/// </summary>
public interface IHasTimeLock
{
    /// <summary>
    /// Checks whether this entity is not locked using <see cref="DateTimeOffset.UtcNow"/> as current time.
    /// </summary>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public bool IsNotLocked();
    
    /// <summary>
    /// Checks whether this entity is not locked at specified time.
    /// </summary>
    /// <param name="now">Specifies the current time to be used when comparing if the entity is locked or not.</param>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public bool IsNotLocked(DateTimeOffset now);
    
    /// <summary>
    /// Sets the <see cref="TimeLock"/> on the entity. This entity will be locked for <paramref name="lockTimeDuration"/> duration.
    /// </summary>
    /// <param name="lockTimeDuration">How long to lock the entity for when using <see cref="DateTimeOffset.UtcNow"/> as current date and time.</param>
    /// <remarks>This does not persist changes in data source where the entity should be locked.
    /// Request to save changes needs to be sent to the data source for this lock to take effect.</remarks>
    /// <returns>True, if this entity was successfully locked,
    /// false if entity cannot be locked now, as there is already lock present on this entity.</returns>
    public bool SetLock(TimeSpan lockTimeDuration);
    
    /// <summary>
    /// Sets the <see cref="TimeLock"/> on the entity. This entity will be locked for specified amount of <paramref name="minutes"/>.
    /// </summary>
    /// <param name="minutes">How long to lock the entity for in minutes when using <see cref="DateTimeOffset.UtcNow"/> as current date and time.</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws this exception if the <paramref name="minutes"/> argument is negative.</exception>
    /// <remarks>This does not persist changes in data source where the entity should be locked.
    /// Request to save changes needs to be sent to the data source for this lock to take effect.</remarks>
    /// <returns>True, if this entity was successfully locked,
    /// false if entity cannot be locked now, as there is already lock present on this entity.</returns>
    public bool SetLock(int minutes);
    
    /// <summary>
    /// Unlocks the <see cref="TimeLock"/> on the entity by setting it to null value.
    /// Useful for when the entity was claimed by current process which has finished operating on it.
    /// Calling this method and persisting changes will make it available for other processes.
    /// </summary>
    /// <remarks>This does not persist changes in data source where the entity should be locked.
    /// Request to save changes needs to be sent to the data source for this lock to take effect.</remarks>
    public void Unlock();

}