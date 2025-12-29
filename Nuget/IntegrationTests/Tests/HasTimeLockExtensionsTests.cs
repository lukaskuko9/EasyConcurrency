using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EasyConcurrency.Tests.IntegrationTests.Tests;

[Collection(DatabaseCollection.CollectionName)]
public class HasTimeLockExtensionsTests : DatabaseFixture
{
    [Fact]
    public async Task WhereIsNotLocked_WorksAsIntended()
    {
        //Arrange
        const int eachCollectionCount = 20;
        var unlockedEntities = Enumerable.Range(0, eachCollectionCount)
            .Select(i => new MyLockableEntity
                {
                    TestParameterGuid = Guid.NewGuid(),
                    TestParameterString = i.ToString(),
                    LockedUntil = null
                }
            )
            .ToList();

        var lockedEntities1Minute = Enumerable.Range(1 * eachCollectionCount, eachCollectionCount)
            .Select(i => new MyLockableEntity
                {
                    TestParameterGuid = Guid.NewGuid(),
                    TestParameterString = i.ToString(),
                    LockedUntil = DateTimeOffset.UtcNow.AddMinutes(1)
                }
            )
            .ToList();

        var lockedEntities5Minutes = Enumerable.Range(2 * eachCollectionCount, eachCollectionCount)
            .Select(i => new MyLockableEntity
                {
                    TestParameterGuid = Guid.NewGuid(),
                    TestParameterString = i.ToString(),
                    LockedUntil = DateTimeOffset.UtcNow.AddMinutes(5)
                }
            )
            .ToList();
        
        Assert.True(lockedEntities1Minute.All(entity=>entity.LockedUntil?.IsLocked() == true));
        Assert.True(lockedEntities5Minutes.All(entity=>entity.LockedUntil?.IsLocked() == true));
        Assert.True(unlockedEntities.All(entity=>entity.LockedUntil.IsNotLocked()));
        await Context.MyLockableEntities.AddRangeAsync(unlockedEntities);
        await Context.MyLockableEntities.AddRangeAsync(lockedEntities1Minute);
        await Context.MyLockableEntities.AddRangeAsync(lockedEntities5Minutes);
        await Context.SaveChangesAsync();
        
        //Act
        var totalNumberOfEntities = await Context.MyLockableEntities.CountAsync();
        var dbNotLockedWithImplicitNow = await Context.MyLockableEntities.WhereIsNotLocked().ToListAsync();
        var dbNotLockedWithExplicitNow = await Context.MyLockableEntities.WhereIsNotLocked(DateTimeOffset.UtcNow).ToListAsync();
        var dbNotLockedIn2Minutes = await Context.MyLockableEntities.WhereIsNotLocked(DateTimeOffset.UtcNow.AddMinutes(2)).ToListAsync();
        //every item should be here as in 6 minutes every lock we set should have expired
        var dbNotLockedIn6Minutes = await Context.MyLockableEntities.WhereIsNotLocked(DateTimeOffset.UtcNow.AddMinutes(6)).ToListAsync();
        
        //Assertions
        
        //Assert all collections unlocked set correctly
        Assert.True(dbNotLockedWithImplicitNow.All(entity => entity.LockedUntil.IsNotLocked()));
        Assert.True(dbNotLockedWithExplicitNow.All(entity => entity.LockedUntil.IsNotLocked()));
        Assert.True(dbNotLockedIn2Minutes.All(entity => entity.LockedUntil.IsNotLocked(DateTimeOffset.UtcNow.AddMinutes(2))));
        Assert.True(dbNotLockedIn6Minutes.Count.Equals(totalNumberOfEntities));
        
        //Assert number of items in each collection is correct
        
        //entities that were not locked should have no lock at the moment
        Assert.Equivalent(dbNotLockedWithImplicitNow, unlockedEntities);
        
        //should be same thing
        Assert.Equivalent(dbNotLockedWithImplicitNow, dbNotLockedWithExplicitNow);
        
        //in 2 minutes, unlocked database entities should be entities we locked for 1 minute and entities we did not lock at all
        Assert.Equivalent(dbNotLockedIn2Minutes, lockedEntities1Minute.Concat(unlockedEntities));
        
        //in 6 minutes, unlocked database entities should be entities we locked for 5 minutes + entities locked for 1 minute + entities we did not lock at all
        Assert.Equivalent(dbNotLockedIn6Minutes, lockedEntities5Minutes.Concat(lockedEntities1Minute.Concat(unlockedEntities)));
        
    }
}