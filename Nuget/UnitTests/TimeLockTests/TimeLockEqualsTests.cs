using EasyConcurrency.Abstractions.Entities;

namespace UnitTests.TimeLockTests;

public partial class TimeLockTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-10)]
    public void EqualsTimeLock(int lockForMinutes)
    {
        var now = DateTimeOffset.UtcNow;
        var timeLock = TimeLock.Create(now.AddMinutes(lockForMinutes));
        var timeLock2 = TimeLock.Create(timeLock);
        var actualResult = timeLock.Equals(timeLock2);
        var expectedResult = timeLock.Value.Equals(timeLock2.Value);
        Assert.Equal(expectedResult, actualResult);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-10)]
    public void EqualsDateTimeOffset(int lockForMinutes)
    {
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(lockForMinutes);
        var timeLock = TimeLock.Create(lockedUntil);
        var actualResult = timeLock.Equals(lockedUntil);
        var expectedResult = timeLock.Value.Equals(lockedUntil);
        Assert.Equal(expectedResult, actualResult);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-10)]
    public void EqualsNullableDateTimeOffset(int? lockForMinutes)
    {
        DateTimeOffset? lockedUntil = lockForMinutes is null 
            ? null 
            : DateTimeOffset.UtcNow.AddMinutes(lockForMinutes.Value);
        
        var timeLock = TimeLock.Create(lockedUntil);
        var actualResult = timeLock.Equals(lockedUntil);
        var expectedResult = timeLock.Value.Equals(lockedUntil);
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-10)]
    public void EqualsNullableTimeLock(int lockForMinutes)
    {
        var now = DateTimeOffset.UtcNow;
        TimeLock? timeLock = TimeLock.Create(now.AddMinutes(lockForMinutes));
        TimeLock? timeLock2 = TimeLock.Create(timeLock);
        var actualResult = timeLock.Equals(timeLock2);
        var expectedResult = timeLock.Value.Equals(timeLock2.Value);
        Assert.Equal(expectedResult, actualResult);
    }
}