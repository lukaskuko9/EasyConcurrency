namespace EasyConcurrency.Abstractions;

public interface ILockableEntity
{
    public TimeLock? LockedUntil { get; set; }
    public bool IsNotLocked();
    public bool IsNotLocked(DateTimeOffset now);
    public bool SetLock(TimeSpan lockTime);
    public bool SetLock(int minutes);
    public void Unlock();
}