namespace EasyConcurrency.Abstractions;

public abstract class LockableEntity : ConcurrentEntity, ILockableEntity
{
    public TimeLock? LockedUntil { get; set; }
    
    public bool IsNotLocked()
    {
        return LockedUntil.IsNotLocked();
    }
    
    public bool IsNotLocked(DateTimeOffset now)
    {
        return LockedUntil.IsNotLocked(now);
    }
    
    public bool SetLock(TimeLock timeLock)
    {
        if (LockedUntil != null) 
            return LockedUntil.Value.SetLock(timeLock);
        
        LockedUntil = TimeLock.Create(timeLock);
        return true;
    }
    
    
    public bool SetLock(TimeSpan lockTimeDuration)
    {
        if (LockedUntil != null) 
            return LockedUntil.Value.SetLock(lockTimeDuration);
        
        LockedUntil = TimeLock.Create(DateTimeOffset.UtcNow.Add(lockTimeDuration));
        return true;
    }
    
    public bool SetLock(int minutes)
    {
        if (LockedUntil != null)
            return LockedUntil.Value.SetLock(minutes);
        
        LockedUntil = TimeLock.Create(DateTimeOffset.UtcNow.AddMinutes(minutes));
        return true;

    }

    public void Unlock()
    { 
        LockedUntil = null;
    }
}