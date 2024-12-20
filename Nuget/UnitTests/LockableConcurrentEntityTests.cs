using EasyConcurrency.Abstractions;
using EasyConcurrency.Abstractions.Entities.LockableEntity;
using EasyConcurrency.Abstractions.TimeLockNamespace;
using Stubs;

namespace UnitTests;

public class LockableConcurrentEntityTests
{
    [Fact]
    public void MyDbEntityIsAssignableToLockableEntity()
    {
        var entity = new MyDbConcurrentEntity { MyUniqueKey = Guid.NewGuid() };
        Assert.IsAssignableFrom<LockableConcurrentEntity>(entity);
    }

    [Fact]
    public void IsLockedTests()
    {
        var entity = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = DateTimeOffset.UtcNow.AddMinutes(10)
        };
        
        Assert.False(entity.IsNotLocked());
        Assert.False(entity.IsNotLocked(DateTimeOffset.UtcNow));
        Assert.False(entity.LockedUntil.IsNotLocked());
    }
    
    [Fact]
    public void IsNotLockedTests()
    {
        var entity = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = null
        };

        Assert.True(entity.IsNotLocked());
        Assert.True(entity.IsNotLocked(DateTimeOffset.UtcNow));
        Assert.True(entity.LockedUntil.IsNotLocked());
    }
    
    [Fact]
    public void SetLockedLocksWhenNotLocked()
    {
        var entity = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = null
        };

        Assert.True(entity.SetLock(5));
        entity.LockedUntil = null;
        
        Assert.True(entity.SetLock(TimeSpan.FromMinutes(5)));
        entity.LockedUntil = null;
        
        Assert.True(entity.SetLock(DateTimeOffset.UtcNow.AddMinutes(5)));
        entity.LockedUntil = null;
    }
    
    [Fact]
    public void SetLockedDoesNotLockWhenLocked()
    {
        var entity = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = DateTimeOffset.UtcNow.AddMinutes(5)
        };
        
        Assert.False(entity.SetLock(5));
        Assert.False(entity.SetLock(TimeSpan.FromMinutes(5)));
        Assert.False(entity.SetLock(DateTimeOffset.UtcNow.AddMinutes(5)));
    }
    
    [Fact]
    public void UnlockSetsLockedUntilToNull()
    {
        var entity = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = DateTimeOffset.UtcNow.AddMinutes(5)
        };
        Assert.NotNull(entity.LockedUntil);
        entity.Unlock();
        Assert.Null(entity.LockedUntil);
    }

}