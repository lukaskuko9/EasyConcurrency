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
        if (IsNotLocked() == false)
            return false;

        LockedUntil = timeLock;
        return true;
    }
    
    
    public bool SetLock(TimeSpan lockTime)
    {
        if (IsNotLocked() == false)
            return false;

        LockedUntil = DateTimeOffset.UtcNow.Add(lockTime);
        return true;
    }
    
    public bool SetLock(int minutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minutes);
        return SetLock(TimeSpan.FromMinutes(minutes));
    }

    public void Unlock()
    { 
        LockedUntil = null;
    }
}