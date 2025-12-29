using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Tests.Shared;

public record struct CustomTimeLock : ITimeLock
{
    public DateTimeOffset? Value { get; set; }
    
    public bool IsNotLocked()
    {
        return IsLocked(DateTimeOffset.UtcNow) == false;
    }

    public bool IsLocked()
    {
        return IsLocked(DateTimeOffset.UtcNow);
    }

    public bool IsNotLocked(DateTimeOffset now)
    {
        return IsLocked(now) == false;
    }

    public bool IsLocked(DateTimeOffset now)
    {
        return now >= Value;
    }

    public int CompareTo(DateTimeOffset other)
    {
        return Nullable.Compare(Value, other);
    }

    public int CompareTo(DateTimeOffset? other)
    {
        return Nullable.Compare(Value, other);
    }

    public bool Equals(DateTimeOffset? other)
    {
        return other.Equals(Value);
    }

    public bool Equals(DateTimeOffset other)
    {
        return other.Equals(Value);
    }

    public static ITimeLock Create(DateTimeOffset? lockedUntil)
    {
        return new CustomTimeLock
        {
            Value = lockedUntil
        };
    }
}