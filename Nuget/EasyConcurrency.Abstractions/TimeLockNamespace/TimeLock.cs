namespace EasyConcurrency.Abstractions.TimeLockNamespace;

/// <summary>
/// Represents a time lock to be held on an entity that naturally expires.
/// </summary>
/// <param name="Value">Time when the lock expires</param>
public record struct TimeLock(DateTimeOffset? Value) : IHasTimeLock, IComparable<DateTimeOffset?>, IComparable<TimeLock?>, IEquatable<DateTimeOffset?>, IEquatable<DateTimeOffset>
{
    /// <summary>
    /// Implicit operator for <see cref="DateTimeOffset"/> and <see cref="TimeLock"/> conversion
    /// </summary>
    /// <param name="timeLock"><paramref name="timeLock"/> value</param>
    /// <returns>New <see cref="DateTimeOffset"/> value</returns>
    public static implicit operator DateTimeOffset?(TimeLock? timeLock) => timeLock?.Value;
    
    /// <summary>
    /// Implicit operator for <see cref="DateTimeOffset"/> and <see cref="TimeLock"/> conversion
    /// </summary>
    /// <param name="timeLock"><paramref name="timeLock"/> value</param>
    /// <returns>New <see cref="DateTimeOffset"/> value</returns>
    public static implicit operator DateTimeOffset?(TimeLock timeLock) => timeLock.Value;
    
    /// <summary>
    /// Implicit operator for <see cref="TimeLock"/> and <see cref="DateTimeOffset"/> conversion
    /// </summary>
    /// <param name="lockedUntil"><paramref name="lockedUntil"/> value</param>
    /// <returns>New <see cref="TimeLock"/> value</returns>
    public static implicit operator TimeLock?(DateTimeOffset? lockedUntil) => lockedUntil == null ? null : new TimeLock(lockedUntil.Value);
    
    /// <summary>
    /// Implicit operator for <see cref="TimeLock"/> and <see cref="DateTimeOffset"/> conversion
    /// </summary>
    /// <param name="lockedUntil"><paramref name="lockedUntil"/> value</param>
    /// <returns>New <see cref="TimeLock"/> value</returns>
    public static implicit operator TimeLock(DateTimeOffset lockedUntil) => new(lockedUntil);
    
    /// <summary>
    /// Creates a <see cref="TimeLock"/> instance. 
    /// </summary>
    /// <param name="lockedUntil">The date and time expiration of the lock</param>
    /// <returns>New TimeLock instance specifying date and time until which the lock takes effect</returns>
    public static TimeLock Create(DateTimeOffset? lockedUntil) => new(lockedUntil);
    
    /// <summary>
    /// Creates a <see cref="TimeLock"/> instance with <paramref name="now"/> as current time
    /// </summary>
    /// <param name="now">Current date and time</param>
    /// <param name="lockTimeDuration">Duration of the lock</param>
    /// <returns>New TimeLock instance specifying date and time until which the lock takes effect</returns>
    public static TimeLock Create(DateTimeOffset now, TimeSpan lockTimeDuration) => new(now.Add(lockTimeDuration));

    /// <inheritdoc />
    public bool IsNotLocked()
    {
        return IsNotLocked(DateTimeOffset.UtcNow);
    }
    
    /// <inheritdoc />
    public bool IsNotLocked(DateTimeOffset now)
    {
        return Value == null || Value < now;
    }

    /// <inheritdoc />
    public bool SetLock(TimeLock timeLock)
    {
        if (IsNotLocked() == false)
            return false;

        Value = timeLock.Value;
        return true;
    }
    
    /// <inheritdoc />
    public bool SetLock(TimeSpan lockTimeDuration)
    {
        if (IsNotLocked() == false)
            return false;

        Value = DateTimeOffset.UtcNow.Add(lockTimeDuration);
        return true;
    }
    
    /// <inheritdoc />
    public bool SetLock(int minutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minutes);
        return SetLock(TimeSpan.FromMinutes(minutes));
    }

    /// <inheritdoc />
    public void Unlock()
    { 
        Value = null;
    }
    
    /// <inheritdoc />
    public int CompareTo(DateTimeOffset? other)
    {
        return Nullable.Compare(Value, other);
    }
    
    /// <inheritdoc />
    public int CompareTo(TimeLock? other)
    {
        return Nullable.Compare(Value, other?.Value);
    }
    
    /// <inheritdoc />
    public bool Equals(DateTimeOffset? other)
    {
        return Value.Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(TimeLock other)
    {
        return Value.Equals(other.Value);
    }

    /// <inheritdoc />
    public bool Equals(DateTimeOffset other)
    {
        return Value.Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}