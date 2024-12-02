namespace EntityFrameworkCore.PessimisticConcurrency.Abstractions;

public readonly record struct TimeLock(DateTimeOffset? Value) : IComparable<DateTimeOffset?>, IComparable<TimeLock?>, IEquatable<DateTimeOffset?>, IEquatable<DateTimeOffset>
{
    public static implicit operator DateTimeOffset?(TimeLock? d) => d?.Value;
    public static implicit operator DateTimeOffset(TimeLock d) => d.Value ?? default;
    public static implicit operator TimeLock?(DateTimeOffset? d) => d == null ? null : new TimeLock(d.Value);
    public static implicit operator TimeLock(DateTimeOffset d) => new(d);
    
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

    public readonly bool Equals(TimeLock other)
    {
        return Value.Equals(other.Value);
    }

    public bool Equals(DateTimeOffset other)
    {
        return Value.Equals(other);
    }

    public readonly override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}