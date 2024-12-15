using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.Entities.LockableEntity;

public abstract class LockableEntity : ILockableEntity
{
    [ConcurrencyCheck]
    public TimeLock.TimeLock? LockedUntil { get; set; }
    
    public bool IsNotLocked()
    {
        return LockedUntil.IsNotLocked();
    }
    
    public bool IsNotLocked(DateTimeOffset now)
    {
        return LockedUntil.IsNotLocked(now);
    }
    
    public bool SetLock(TimeLock.TimeLock timeLock)
    {
        if (LockedUntil != null) 
            return LockedUntil.Value.SetLock(timeLock);
        
        LockedUntil = TimeLock.TimeLock.Create(timeLock);
        return true;
    }
    
    
    public bool SetLock(TimeSpan lockTimeDuration)
    {
        if (LockedUntil != null) 
            return LockedUntil.Value.SetLock(lockTimeDuration);
        
        LockedUntil = TimeLock.TimeLock.Create(DateTimeOffset.UtcNow.Add(lockTimeDuration));
        return true;
    }
    
    public bool SetLock(int minutes)
    {
        if (LockedUntil != null)
            return LockedUntil.Value.SetLock(minutes);
        
        LockedUntil = TimeLock.TimeLock.Create(DateTimeOffset.UtcNow.AddMinutes(minutes));
        return true;

    }

    public void Unlock()
    { 
        LockedUntil = null;
    }
}