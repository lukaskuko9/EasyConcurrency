namespace EasyConcurrency.Abstractions.TimeLock;

/// <summary>
/// Represents a time lock to be held on an entity that naturally expires.
/// </summary>
public record struct TimeLock : ITimeLock, IComparable<TimeLock?>, IComparable<TimeLock>
{
    /// <summary>
    /// Represents a time lock to be held on an entity that naturally expires.
    /// </summary>
    /// <param name="Value">DatetimeOffset as a point in time, indicating when the lock expires. Null if lock is not set.</param>
    public TimeLock(DateTimeOffset? Value) => this.Value = Value;

    ///<inheritdoc />
    public DateTimeOffset? Value { get; set; }

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
    
    /// <inheritdoc/>
    static ITimeLock ITimeLock.Create(DateTimeOffset? lockedUntil)
    {
        return Create(lockedUntil);
    }
    
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
        return IsLocked() == false;
    }
    
    /// <inheritdoc />
    public bool IsNotLocked(DateTimeOffset now)
    {
        return IsLocked(now) == false;
    }

    /// <inheritdoc />
    public bool IsLocked()
    {
        return IsLocked(DateTimeOffset.UtcNow);
    }

    /// <inheritdoc />
    public bool IsLocked(DateTimeOffset now)
    {
        return Value >= now;
    }

    /// <inheritdoc />
    public int CompareTo(DateTimeOffset other)
    {
        return Nullable.Compare(Value, other);
    }
        
    /// <inheritdoc />
    public int CompareTo(DateTimeOffset? other)
    {
        return Nullable.Compare(Value, other);
    }

    
    /// <inheritdoc />
    public int CompareTo(TimeLock other)
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
    public readonly bool Equals(TimeLock other)
    {
        return Nullable.Equals(Value, other.Value);
    }

    /// <inheritdoc />
    public bool Equals(DateTimeOffset other)
    {
        return Value.Equals(other);
    }

    /// <inheritdoc />
    public readonly override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}