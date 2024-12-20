namespace EasyConcurrency.Abstractions.TimeLockNamespace;

/// <summary>
/// Represents a time lock to be held on an entity that naturally expires.
/// </summary>
/// <param name="Value">Time when the lock expires</param>
public record struct TimeLock(DateTimeOffset? Value) : IHasTimeLock, IComparable<DateTimeOffset?>, IComparable<TimeLock?>, IEquatable<DateTimeOffset?>, IEquatable<DateTimeOffset>
{
    public static implicit operator DateTimeOffset?(TimeLock? timeLock) => timeLock?.Value;
    public static implicit operator DateTimeOffset?(TimeLock timeLock) => timeLock.Value;
    public static implicit operator TimeLock?(DateTimeOffset? lockedUntil) => lockedUntil == null ? null : new TimeLock(lockedUntil.Value);
    public static implicit operator TimeLock(DateTimeOffset lockedUntil) => new(lockedUntil);
    
    /// <summary>
    /// Creates a <see cref="TimeLock"/> instance. 
    /// </summary>
    /// <param name="lockedUntil">The date and time expiration of the lock</param>
    /// <returns></returns>
    public static TimeLock Create(DateTimeOffset? lockedUntil) => new(lockedUntil);
    
    public bool IsNotLocked()
    {
        return IsNotLocked(DateTimeOffset.UtcNow);
    }
    
    public bool IsNotLocked(DateTimeOffset now)
    {
        return Value == null || Value < now;
    }

    public bool SetLock(TimeLock timeLock)
    {
        if (IsNotLocked() == false)
            return false;

        Value = timeLock.Value;
        return true;
    }
    
    
    public bool SetLock(TimeSpan lockTimeDuration)
    {
        if (IsNotLocked() == false)
            return false;

        Value = DateTimeOffset.UtcNow.Add(lockTimeDuration);
        return true;
    }
    
    public bool SetLock(int minutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minutes);
        return SetLock(TimeSpan.FromMinutes(minutes));
    }

    public void Unlock()
    { 
        Value = null;
    }

    public int CompareTo(DateTimeOffset? other)
    {
        return Nullable.Compare(Value, other);
    }

    public int CompareTo(TimeLock? other)
    {
        return Nullable.Compare(Value, other?.Value);
    }

    public bool Equals(DateTimeOffset? other)
    {
        return Value.Equals(other);
    }

    public bool Equals(TimeLock other)
    {
        return Value.Equals(other.Value);
    }

    public bool Equals(DateTimeOffset other)
    {
        return Value.Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}