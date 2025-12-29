using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Tests.Shared;

public record struct MyTimeLock : ITimeLock
{
    public DateTimeOffset? Value { get; set; }
    
    public bool IsNotLocked()
    {
        return IsLocked() == false;
    }

    public bool IsLocked()
    {
        return DateTimeOffset.Now >= Value;
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
        throw new NotImplementedException();
    }

    public bool Equals(DateTimeOffset other)
    {
        throw new NotImplementedException();
    }

    public static ITimeLock Create(DateTimeOffset? lockedUntil)
    {
        throw new NotImplementedException();
    }

    public bool IsNotLocked(DateTimeOffset now)
    {
        throw new NotImplementedException();
    }

    public bool IsLocked(DateTimeOffset now)
    {
        throw new NotImplementedException();
    }
}