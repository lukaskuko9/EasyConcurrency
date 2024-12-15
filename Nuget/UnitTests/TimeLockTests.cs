using EasyConcurrency.Abstractions;
using EasyConcurrency.Abstractions.TimeLock;

namespace UnitTests;

public class TimeLockTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(1)]
    [InlineData(0)]
    public void IsLocked(int lockedForMinutes)
    {
        var now = DateTimeOffset.UtcNow;
        var timeLock = TimeLock.Create(now.AddMinutes(lockedForMinutes));
        Assert.False(timeLock.IsNotLocked(now));
    }
    
    [Theory]
    [InlineData(-10)]
    [InlineData(-5)]
    [InlineData(-1)]
    public void IsNotLocked(int lockedForMinutes)
    {
        var now = DateTimeOffset.UtcNow;
        var timeLock = TimeLock.Create(now.AddMinutes(lockedForMinutes));
        Assert.True(timeLock.IsNotLocked(now));
    }
    
    [Fact]
    public void IsNotLockedWhenTimeLockIsNull()
    {
        TimeLock? timeLock = null;
        Assert.True(timeLock.IsNotLocked());
    }
    
    [Fact]
    public void IsNotLockedWhenTimeLockIsNullAndTimeIsProvided()
    {
        TimeLock? timeLock = null;
        Assert.True(timeLock.IsNotLocked(DateTimeOffset.UtcNow));
    }
}