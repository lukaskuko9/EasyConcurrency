using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Abstractions.TimeLock;
using EasyConcurrency.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EasyConcurrency.Tests.IntegrationTests.Tests;

[Collection(DatabaseCollection.CollectionName)]
public class LockTests : DatabaseFixture
{
    [Fact]
    public async Task LockedUntilTranslatesCorrectly()
    {
        var newEntityNotLocked = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = null
        };
        
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        var newEntityLocked = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = lockedUntil
        };
        
        await Context.HasTimeLockEntities.AddAsync(newEntityNotLocked);
        await Context.HasTimeLockEntities.AddAsync(newEntityLocked);
        await Context.SaveChangesAsync();
        
        var dbEntityNotLocked = await Context.HasTimeLockEntities.SingleAsync(myDbEntity => myDbEntity.TestParameterString == newEntityNotLocked.TestParameterString);
        Assert.True(dbEntityNotLocked.LockedUntil.IsNotLocked());
        
        var dbEntityLocked = await Context.HasTimeLockEntities.SingleAsync(myDbEntity => myDbEntity.TestParameterString == newEntityLocked.TestParameterString);
        Assert.False(dbEntityLocked.LockedUntil.IsNotLocked());
        Assert.Equal(dbEntityLocked.LockedUntil, lockedUntil);
    }

    [Fact]
    public async Task LockIsRespected()
    {
        var newEntityNotLocked = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = null
        };
        
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        var newEntityLocked = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = lockedUntil
        };
        
        await Context.HasTimeLockEntities.AddAsync(newEntityNotLocked);
        await Context.HasTimeLockEntities.AddAsync(newEntityLocked);
        await Context.SaveChangesAsync();
        
        var dbEntityNotLocked = await Context.HasTimeLockEntities
            .WhereIsNotLocked()
            .SingleAsync(myDbEntity => myDbEntity.TestParameterString == newEntityNotLocked.TestParameterString);
        
        var dbEntityLockExpired = await Context.HasTimeLockEntities
            .WhereIsNotLocked(DateTimeOffset.UtcNow.AddMinutes(30))
            .SingleOrDefaultAsync(myDbEntity => myDbEntity.TestParameterString == newEntityLocked.TestParameterString);
        
        var dbEntityLocked = await Context.HasTimeLockEntities
            .WhereIsNotLocked()
            .SingleOrDefaultAsync(myDbEntity => myDbEntity.TestParameterString == newEntityLocked.TestParameterString);
        
        //not locked entity can be fetched and is not locked
        Assert.True(dbEntityNotLocked.LockedUntil.IsNotLocked());

        //locked entity cannot be fetched as it is locked
        Assert.Null(dbEntityLocked);
        
        //when lock has expired, entity that was locked is automatically unlocked now and can be fetched
        Assert.NotNull(dbEntityLockExpired);
        Assert.Equal(dbEntityLockExpired.LockedUntil, lockedUntil);
    }

    [Fact]
    public async Task CanBeLocked()
    {
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        var newEntity1 = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = new TimeLock(lockedUntil)
        };
        
        var newEntity2 = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = lockedUntil
        };
        
        var newEntity3 = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = null
        };
        
        var newEntity4 = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = null
        };
        
        await Context.HasTimeLockEntities.AddAsync(newEntity1);
        await Context.HasTimeLockEntities.AddAsync(newEntity2);
        await Context.HasTimeLockEntities.AddAsync(newEntity3);
        await Context.HasTimeLockEntities.AddAsync(newEntity4);
        await Context.SaveChangesAsync();

        var isLocked = newEntity3.LockedUntil = lockedUntil;
        var isLocked2 = newEntity4.LockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        Assert.True(isLocked.IsLocked());
        Assert.True(isLocked2.IsLocked());
        await Context.SaveChangesAsync();

        var notLockedItems = await Context.HasTimeLockEntities
            .WhereIsNotLocked()
            .ToListAsync();
        
        Assert.Empty(notLockedItems);
    }

    [Fact]
    public async Task LockCanBeUnlocked()
    {
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(-10);
        var newEntity = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString(),
            LockedUntil = new TimeLock(lockedUntil)
        };
        await Context.HasTimeLockEntities.AddAsync(newEntity);
        await Context.SaveChangesAsync();

        newEntity.LockedUntil = null;
        await Context.SaveChangesAsync();
        
        Assert.True(newEntity.LockedUntil.IsNotLocked());
        Assert.False(newEntity.LockedUntil.IsLocked());
        
        Assert.True(newEntity.LockedUntil?.IsNotLocked() == null);
        Assert.True(newEntity.LockedUntil?.IsLocked() == null);
    }
}
