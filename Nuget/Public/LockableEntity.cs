namespace Public;

public abstract class LockableEntity : ConcurrentEntity
{
    public TimeLock? LockedUntil { get; set; }
    
    public bool IsNotLocked()
    {
        return IsNotLocked(DateTimeOffset.Now);
    }
    
    public bool IsNotLocked(DateTimeOffset now)
    {
        return LockedUntil == null || LockedUntil < now;
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