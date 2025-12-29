using EasyConcurrency.Tests.Shared;
using Xunit;

namespace EasyConcurrency.Tests.IntegrationTests.Tests;

[Collection(DatabaseCollection.CollectionName)]
public class CustomTimeLockEntityTests : DatabaseFixture
{
    [Fact]
    public async Task CustomImplementationOfTimeLockCanBeLockedAndUnlocked()
    {
        var entity = new CustomTimeLockEntity
        {
            TestParameterString = "TestParameterString",
            LockedUntil = null
        };

        //Save not locked
        await Context.CustomTimeLockEntities.AddAsync(entity);
        await Context.SaveChangesAsync();
        Assert.Null(entity.LockedUntil);
        
        //Lock entity, save and assert
        var lockedUntilTime = DateTimeOffset.UtcNow.AddHours(1);
        entity.LockedUntil = new CustomTimeLock
        {
            Value = lockedUntilTime
        };

        await Context.SaveChangesAsync();
        
        Assert.NotNull(entity.LockedUntil);
        Assert.True(entity.LockedUntil.Value.IsNotLocked());
        Assert.Equal(entity.LockedUntil.Value.Value, lockedUntilTime);
        
        //Unlock entity, save and assert
        entity.LockedUntil = null;
        await Context.SaveChangesAsync();
        
        Assert.Null(entity.LockedUntil);
    }
}