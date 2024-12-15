using EasyConcurrency.Abstractions.TimeLock;
using EasyConcurrency.EntityFramework.LockableEntity;
using Microsoft.EntityFrameworkCore;
using Stubs;
using Xunit;

namespace IntegrationTests.Tests;

[Collection(DatabaseCollection.CollectionName)]
public class LockTests : DatabaseFixture
{
    [Fact]
    public async Task LockedUntilTranslatesCorrectly()
    {
        var newEntityNotLocked = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = null
        };
        
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        var newEntityLocked = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = lockedUntil
        };
        
        await Context.MyDbEntities.AddAsync(newEntityNotLocked);
        await Context.MyDbEntities.AddAsync(newEntityLocked);
        await Context.SaveChangesAsync();
        
        var dbEntityNotLocked = await Context.MyDbEntities.SingleAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityNotLocked.MyUniqueKey);
        Assert.True(dbEntityNotLocked.IsNotLocked());
        
        var dbEntityLocked = await Context.MyDbEntities.SingleAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityLocked.MyUniqueKey);
        Assert.False(dbEntityLocked.IsNotLocked());
        Assert.Equal(dbEntityLocked.LockedUntil, lockedUntil);
    }

    [Fact]
    public async Task LockIsRespected()
    {
        var newEntityNotLocked = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = null
        };
        
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        var newEntityLocked = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = lockedUntil
        };
        
        await Context.MyDbEntities.AddAsync(newEntityNotLocked);
        await Context.MyDbEntities.AddAsync(newEntityLocked);
        await Context.SaveChangesAsync();
        
        var dbEntityNotLocked = await Context.MyDbEntities
            .WhereIsNotLocked()
            .SingleAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityNotLocked.MyUniqueKey);
        
        var dbEntityLockExpired = await Context.MyDbEntities
            .WhereIsNotLocked(DateTimeOffset.UtcNow.AddMinutes(30))
            .SingleOrDefaultAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityLocked.MyUniqueKey);
        
        var dbEntityLocked = await Context.MyDbEntities
            .WhereIsNotLocked()
            .SingleOrDefaultAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityLocked.MyUniqueKey);
        
        //not locked entity can be fetched and is not locked
        Assert.True(dbEntityNotLocked.IsNotLocked());

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
        var newEntity1 = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = new TimeLock(lockedUntil)
        };
        
        var newEntity2 = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = lockedUntil
        };
        
        var newEntity3 = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = null
        };
        
        var newEntity4 = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = null
        };
        
        await Context.MyDbEntities.AddAsync(newEntity1);
        await Context.MyDbEntities.AddAsync(newEntity2);
        await Context.MyDbEntities.AddAsync(newEntity3);
        await Context.MyDbEntities.AddAsync(newEntity4);
        await Context.SaveChangesAsync();

        var isLocked = newEntity3.SetLock(lockedUntil);
        var isLocked2 = newEntity4.SetLock(TimeSpan.FromMinutes(10));
        Assert.True(isLocked);
        Assert.True(isLocked2);
        await Context.SaveChangesAsync();

        var notLockedItems = await Context.MyDbEntities
            .WhereIsNotLocked()
            .ToListAsync();
        
        Assert.Empty(notLockedItems);
    }

    [Fact]
    public async Task LockCanBeUnlocked()
    {
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(-10);
        var newEntity = new MyDbConcurrentEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = new TimeLock(lockedUntil)
        };
        await Context.MyDbEntities.AddAsync(newEntity);
        await Context.SaveChangesAsync();

        newEntity.Unlock();
        await Context.SaveChangesAsync();
        
        Assert.True(newEntity.IsNotLocked());
    }
}
