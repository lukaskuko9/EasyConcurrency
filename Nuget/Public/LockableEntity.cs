namespace Public;

public abstract class LockableEntity : ConcurrentEntity
{
    public TimeLock? LockedUntil { get; set; }
    
    public bool IsNotLocked()
    {
        return LockedUntil == null || LockedUntil < DateTimeOffset.Now;
    }
    
    public bool SetLock(TimeSpan lockTime)
    {
        if (IsNotLocked() == false)
            return false;
        
        LockedUntil = DateTimeOffset.Now.Add(lockTime);
        return true;
    }
    
    public bool SetLock(int minutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minutes);
        return SetLock(TimeSpan.FromMinutes(minutes));
    }
}