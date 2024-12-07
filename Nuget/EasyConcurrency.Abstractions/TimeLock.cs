namespace EasyConcurrency.Abstractions;

/// <summary>
/// Represents a time lock to be held on an entity that naturally expires.
/// </summary>
/// <param name="Value">Time when the lock expires</param>
public readonly record struct TimeLock(DateTimeOffset? Value) : IIsNotLocked, IComparable<DateTimeOffset?>, IComparable<TimeLock?>, IEquatable<DateTimeOffset?>, IEquatable<DateTimeOffset>
{
    public static implicit operator DateTimeOffset?(TimeLock? timeLock) => timeLock?.Value;
    public static implicit operator DateTimeOffset(TimeLock timeLock) => timeLock.Value ?? default;
    public static implicit operator TimeLock?(DateTimeOffset? lockedUntil) => lockedUntil == null ? null : Create(lockedUntil.Value);
    public static implicit operator TimeLock(DateTimeOffset lockedUntil) => Create(lockedUntil);
    /// <summary>
    /// Creates a <see cref="TimeLock"/> instance. 
    /// </summary>
    /// <param name="lockedUntil">The date and time expiration of the lock</param>
    /// <returns></returns>
    public static TimeLock Create(DateTimeOffset lockedUntil) => new(lockedUntil);
    
    public bool IsNotLocked()
    {
        return IsNotLocked(DateTimeOffset.UtcNow);
    }
    
    public bool IsNotLocked(DateTimeOffset now)
    {
        return Value == null || Value < now;
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