using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.Extensions;

namespace EasyConcurrency.Abstractions.Entities;

/// <summary>
/// Provides base methods for handling pessimistic concurrency scenarios of implementing class instance. 
/// </summary>
public abstract class TimeLockEntity : ITimeLockEntity
{
    /// <inheritdoc />
    [ConcurrencyCheck]
    public TimeLock? LockedUntil { get; set; }
    
    /// <inheritdoc />
    public bool IsNotLocked()
    {
        return LockedUntil.IsNotLocked();
    }
        
    /// <inheritdoc />
    public bool IsNotLocked(DateTimeOffset now)
    {
        return LockedUntil.IsNotLocked(now);
    }
        
    /// <inheritdoc />
    public bool SetLock(TimeLock timeLock)
    {
        if (LockedUntil != null) 
            return LockedUntil.Value.SetLock(timeLock);
        
        LockedUntil = TimeLock.Create(timeLock);
        return true;
    }
    
    /// <inheritdoc />
    public bool SetLock(TimeSpan lockTimeDuration)
    {
        if (LockedUntil != null) 
            return LockedUntil.Value.SetLock(lockTimeDuration);
        
        LockedUntil = TimeLock.Create(DateTimeOffset.UtcNow.Add(lockTimeDuration));
        return true;
    }
        
    /// <inheritdoc />
    public bool SetLock(int minutes)
    {
        if (LockedUntil != null)
            return LockedUntil.Value.SetLock(minutes);
        
        LockedUntil = TimeLock.Create(DateTimeOffset.UtcNow.AddMinutes(minutes));
        return true;
    }
    
    /// <inheritdoc />
    public void Unlock()
    { 
        LockedUntil = null;
    }
}