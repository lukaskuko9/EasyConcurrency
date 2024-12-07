namespace EasyConcurrency.Abstractions;

public readonly record struct TimeLock(DateTimeOffset? Value) : IComparable<DateTimeOffset?>, IComparable<TimeLock?>, IEquatable<DateTimeOffset?>, IEquatable<DateTimeOffset>
{
    public static implicit operator DateTimeOffset?(TimeLock? timeLock) => timeLock?.Value;
    public static implicit operator DateTimeOffset(TimeLock timeLock) => timeLock.Value ?? default;
    public static implicit operator TimeLock?(DateTimeOffset? timeLock) => timeLock == null ? null : Create(timeLock.Value);
    public static implicit operator TimeLock(DateTimeOffset timeLock) => Create(timeLock);
    
    public static TimeLock Create(DateTimeOffset timeLock) => new(timeLock);
    
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