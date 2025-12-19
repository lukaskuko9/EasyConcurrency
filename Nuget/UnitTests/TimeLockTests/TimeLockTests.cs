using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Abstractions.TimeLock;

namespace UnitTests.TimeLockTests;

public partial class TimeLockTests
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
        Assert.True(timeLock.IsLocked(now));
        Assert.True(timeLock2.IsLocked(now));
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
        Assert.False(timeLock.IsLocked(now));
        Assert.False(timeLock2.IsLocked(now));
        Assert.Equal(timeLock.Value, timeLock2.Value);
    }
    
    [Fact]
    public void IsNotLockedWhenTimeLockIsNull()
    {
        TimeLock? timeLock = null;
        Assert.True(timeLock.IsNotLocked());
        Assert.False(timeLock.IsLocked());
    }
    
    [Fact]
    public void IsNotLockedWhenTimeLockIsNullAndTimeIsProvided()
    {
        TimeLock? timeLock = null;
        var now = DateTimeOffset.UtcNow;
        Assert.True(timeLock.IsNotLocked(now));
        Assert.False(timeLock.IsLocked(now));
    }
    
    [Fact]
    public void UnlockTest()
    {
        var timeLock = TimeLock.Create(DateTimeOffset.UtcNow.AddMinutes(10));
        Assert.False(timeLock.IsNotLocked());
        Assert.True(timeLock.IsLocked());
        
        timeLock.Unlock();
        
        Assert.True(timeLock.IsNotLocked());
        Assert.False(timeLock.IsLocked());
    }
        
    [Fact]
    public void ValueSetTest()
    {
        var timeLock = new TimeLock();
        Assert.Null(timeLock.Value);
        
        timeLock.Value = DateTimeOffset.UtcNow.AddMinutes(10);
        Assert.False(timeLock.IsNotLocked());
        Assert.True(timeLock.IsLocked());
    }
    
    [Fact]
    public void SetLockTestUsingAnotherTimeLock()
    {
        var timeLock = TimeLock.Create(null);
        Assert.True(timeLock.IsNotLocked());
        Assert.False(timeLock.IsLocked());

        timeLock.SetLock(new TimeLock(DateTimeOffset.UtcNow.AddMinutes(10)));
        Assert.False(timeLock.IsNotLocked());
        Assert.True(timeLock.IsLocked());
    }
    
    [Fact]
    public void SetLockTestUsingImplicitConversionFromNullableDateTimeOffset()
    {
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        TimeLock? timeLock = lockedUntil;
        DateTimeOffset? lockedUntilTakenFromTimeLock = timeLock;
        Assert.Equal(lockedUntil, lockedUntilTakenFromTimeLock);
    }
    
    
    [Fact]
    public void SetLockTestUsingTimeSpan()
    {
        var timeLock = TimeLock.Create(null);
        Assert.True(timeLock.IsNotLocked());
        Assert.False(timeLock.IsLocked());

        timeLock.SetLock(TimeSpan.FromMinutes(10));
        Assert.False(timeLock.IsNotLocked());
        Assert.True(timeLock.IsLocked());
    }
    
    [Fact]
    public void SetLockTestUsingMinutesArgument()
    {
        var timeLock = TimeLock.Create(null);
        Assert.True(timeLock.IsNotLocked());
        Assert.False(timeLock.IsLocked());

        timeLock.SetLock(DateTimeOffset.UtcNow.AddMinutes(10));
        Assert.False(timeLock.IsNotLocked());
        Assert.True(timeLock.IsLocked());
    }
    
    [Fact]
    public void ImplicitConversionToDateTimeOffsetTest()
    {
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        TimeLock timeLock = lockedUntil;
        Assert.False(timeLock.IsNotLocked()); 
        Assert.True(timeLock.IsLocked());       
    }
    
    [Fact]
    public void ImplicitConversionToDateTimeOffsetAndBack()
    {
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        TimeLock? timeLock = lockedUntil;
        DateTimeOffset? lockedUntilTakenFromTimeLock = timeLock;
        Assert.Equal(lockedUntil, lockedUntilTakenFromTimeLock);
        TimeLock? timeLock2 = lockedUntilTakenFromTimeLock;
        
        Assert.False(timeLock2.IsNotLocked());
        Assert.True(timeLock2.IsLocked());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData(-10)]
    public void ImplicitConversionToNullableDateTimeOffsetTest(int? lockForMinutes)
    {
        DateTimeOffset? lockedUntil = lockForMinutes == null ? null : DateTimeOffset.UtcNow.AddMinutes(lockForMinutes.Value);
        TimeLock? timeLock = lockedUntil;
        Assert.True(timeLock.IsNotLocked());      
        Assert.False(timeLock.IsLocked());  
    }

    [Fact]
    public void GetHashCodeIsSameForSameTimestamp()
    {
        var now = DateTimeOffset.UtcNow;
        var timeLock = TimeLock.Create(now);
        var timeLock2 = TimeLock.Create(now);
        Assert.Equal(timeLock.GetHashCode(), timeLock2.GetHashCode());
    }
}