using System.ComponentModel.DataAnnotations;
using Core.PrimitiveTypeObsession;

namespace Infrastructure.Database.Entities;

public abstract record LockableEntity
{
    public TimeLock? LockedUntil { get; set; }

    [Timestamp] 
    public byte[] Version { get; init; } = null!;
    
    public bool IsNotLocked()
    {
        return LockedUntil == null || LockedUntil < DateTimeOffset.Now;
    }
    
    public bool SetLock(TimeSpan lockTime)
    {
        //locked; can't lock; return false
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