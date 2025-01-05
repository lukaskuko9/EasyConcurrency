using EasyConcurrency.Abstractions.Entities;

namespace UnitTests.TimeLockTests;

public partial class TimeLockTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-10)]
    public void CompareToTimeLock(int lockForMinutes)
    {
        var now = DateTimeOffset.UtcNow;
        var timeLock = TimeLock.Create(now);
        var timeLock2 = TimeLock.Create(now.AddMinutes(lockForMinutes));
        var actualResult = timeLock.CompareTo(timeLock2);
        var expectedResult = timeLock.Value?.CompareTo(timeLock2.Value!.Value);
        Assert.Equal(expectedResult, actualResult);
    }


    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-10)]
    public void CompareToNullableTimeLock(int? lockForMinutes)
    {
        var now = DateTimeOffset.UtcNow;
        DateTimeOffset? lockedUntil = lockForMinutes == null ? null : now.AddMinutes(lockForMinutes.Value);
        var timeLock = TimeLock.Create(now);
        TimeLock? timeLock2 = TimeLock.Create(lockedUntil);
        var actualResult = timeLock.CompareTo(timeLock2);
        var expectedResult = timeLock2.Value.Value == null ? 1 : timeLock.Value?.CompareTo(timeLock2.Value.Value.Value);
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-10)]
    public void CompareToDateTimeOffsetTest(int lockForMinutes)
    {
        var now = DateTimeOffset.UtcNow.AddMinutes(lockForMinutes);
        
        var timeLock = TimeLock.Create(now);
        var actualResult = timeLock.CompareTo(now);
        var expectedResult = timeLock.Value?.CompareTo(now);
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-10)]
    public void CompareToNullableDateTimeOffsetTest(int? lockForMinutes)
    {
        DateTimeOffset? now = lockForMinutes == null
            ? null
            : DateTimeOffset.UtcNow.AddMinutes(lockForMinutes.Value);
        
        var timeLock = TimeLock.Create(now);
        var actualResult = timeLock.CompareTo(now);
        var expectedResult = now == null ? 0 : timeLock.Value?.CompareTo(now.Value);
        Assert.Equal(expectedResult, actualResult);
    }
}