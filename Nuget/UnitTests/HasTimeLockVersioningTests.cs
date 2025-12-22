using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Abstractions.HasTimeLock;
using Stubs;

namespace UnitTests;

public class HasTimeLockVersioningTests
{
    [Fact]
    public void MyDbEntityIsAssignableToLockableEntity()
    {
        var entity = new MyLockableEntity() { TestParameterGuid = Guid.NewGuid() };
        Assert.IsAssignableFrom<IHasTimeLock>(entity);
    }

    [Fact]
    public void IsLockedTests()
    {
        var entity = new MyLockableEntity
        {
            TestParameterGuid = Guid.NewGuid(),
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
        var entity = new MyLockableEntity
        {
            TestParameterGuid = Guid.NewGuid(),
            LockedUntil = null
        };

        Assert.True(entity.LockedUntil.IsNotLocked());
        Assert.True(entity.LockedUntil.IsNotLocked(DateTimeOffset.UtcNow));
        
        Assert.False(entity.LockedUntil.IsLocked());
        Assert.False(entity.LockedUntil.IsLocked(DateTimeOffset.UtcNow));
    }

}