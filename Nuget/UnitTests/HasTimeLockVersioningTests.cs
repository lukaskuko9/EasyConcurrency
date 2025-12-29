using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Abstractions.TimeLock;
using EasyConcurrency.Tests.Shared;

namespace EasyConcurrency.Tests.UnitTests;

public class HasTimeLockVersioningTests
{
    [Fact]
    public void MyDbEntityIsAssignableToLockableEntity()
    {
        var entity = new HasTimeLockEntity();
        Assert.IsAssignableFrom<IHasTimeLock>(entity);
    }

    [Fact]
    public void IsLockedTests()
    {
        var entity = new HasTimeLockEntity
        {
            LockedUntil = DateTimeOffset.UtcNow.AddMinutes(10)
        };
        var now = DateTimeOffset.UtcNow;
        Assert.False(entity.LockedUntil?.IsNotLocked());
        Assert.False(entity.LockedUntil?.IsNotLocked(now));
        
        Assert.True(entity.LockedUntil?.IsLocked());
        Assert.True(entity.LockedUntil?.IsLocked(now));
    }
    
    [Fact]
    public void IsNotLockedTests()
    {
        var entity = new HasTimeLockEntity
        {
            LockedUntil = null
        };

        Assert.True(entity.LockedUntil.IsNotLocked());
        Assert.True(entity.LockedUntil.IsNotLocked(DateTimeOffset.UtcNow));
        
        Assert.False(entity.LockedUntil.IsLocked());
        Assert.False(entity.LockedUntil.IsLocked(DateTimeOffset.UtcNow));
    }

}