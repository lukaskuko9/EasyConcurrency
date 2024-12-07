namespace EasyConcurrency.Abstractions;

/// <summary>
/// Provides interface for handling pessimistic concurrency scenarios of implementing class instance. 
/// </summary>
public interface ILockableEntity : IIsNotLocked
{
    /// <summary>
    /// Specifies the <see cref="TimeLock"/> until which the entity remains locked.
    /// If this property points to the future date and / or time,
    /// it is  locked, otherwise it is not locked.
    /// </summary>
    public TimeLock? LockedUntil { get; set; }
    
    /// <summary>
    /// Sets the <see cref="TimeLock"/> on the entity. This entity will be locked for <paramref name="lockTime"/> duration.
    /// </summary>
    /// <param name="lockTime">How long to lock the entity for.</param>
    /// <remarks>This does not persist changes in data source where the entity should be locked.
    /// Request to save changes needs to be sent to the data source for this lock to take effect.</remarks>
    /// <returns>True, if this entity was successfully locked,
    /// false if entity cannot be locked now, as there is already lock present on this entity.</returns>
    public bool SetLock(TimeSpan lockTime);
    
    /// <summary>
    /// Sets the <see cref="TimeLock"/> on the entity. This entity will be locked for specified amount of <paramref name="minutes"/>.
    /// </summary>
    /// <param name="minutes">How long to lock the entity for in minutes.</param>
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