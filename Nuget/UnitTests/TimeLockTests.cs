using EasyConcurrency.Abstractions;
using EasyConcurrency.Abstractions.TimeLockNamespace;

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
        var timeLock2 = TimeLock.Create(now, TimeSpan.FromMinutes(lockedForMinutes));
        Assert.False(timeLock.IsNotLocked(now));
        Assert.False(timeLock2.IsNotLocked(now));
        Assert.Equal(timeLock.Value, timeLock2.Value);
    }
    
    [Theory]
    [InlineData(-10)]
    [InlineData(-5)]
    [InlineData(-1)]
    public void IsNotLocked(int lockedForMinutes)
    {
        var now = DateTimeOffset.UtcNow;
        var timeLock = TimeLock.Create(now.AddMinutes(lockedForMinutes));
        var timeLock2 = TimeLock.Create(now, TimeSpan.FromMinutes(lockedForMinutes));
        Assert.True(timeLock.IsNotLocked(now));
        Assert.True(timeLock2.IsNotLocked(now));
        Assert.Equal(timeLock.Value, timeLock2.Value);
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