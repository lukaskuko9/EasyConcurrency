namespace EasyConcurrency.Abstractions;

public abstract class LockableEntity : ConcurrentEntity, ILockableEntity
{
    public TimeLock? LockedUntil { get; set; }
    
    public bool IsNotLocked()
    {
        return IsNotLocked(DateTimeOffset.UtcNow);
    }
    
    public bool IsNotLocked(DateTimeOffset now)
    {
        return TimeLockMethods.IsNotLocked(this);
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
}