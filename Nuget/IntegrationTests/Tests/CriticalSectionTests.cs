using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.EntityFramework.CriticalSection;
using EasyConcurrency.Tests.IntegrationTests.Database;
using EasyConcurrency.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EasyConcurrency.Tests.IntegrationTests.Tests;

[Collection(DatabaseCollection.CollectionName)]
public class CriticalSectionTests(ITestOutputHelper logger) : DatabaseFixture
{
    [Fact]
    public async Task BeginCriticalSection_NoConcurrency()
    {
        //Arrange
        var criticalSectionService = new CriticalSectionService<DatabaseContext>(Context);
        var expectedEntity = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString()
        };
        await Context.HasTimeLockEntities.AddAsync(expectedEntity);
        await Context.SaveChangesAsync();
        
        //Act
        await using (var criticalSection = await criticalSectionService.BeginCriticalSectionAndCommitAsync(expectedEntity, TimeSpan.FromMinutes(1)))
        {
            if (criticalSection.IsLockAcquired == false || expectedEntity.LockedUntil?.IsLocked() == false)
            {
                Assert.Fail("The entity is NOT locked");
            }
            
            //Assert that entity should be locked here
            var entity = await Context.HasTimeLockEntities.SingleAsync(x=>x.Id == expectedEntity.Id);
            Assert.NotNull(entity.LockedUntil);
            Assert.True(entity.LockedUntil.Value.Value > DateTimeOffset.Now);
        }
        
        //Assert
        var actualEntity = await Context.HasTimeLockEntities.SingleAsync(x=>x.Id == expectedEntity.Id);
        Assert.Null(actualEntity.LockedUntil);
        Assert.True(actualEntity.LockedUntil.IsNotLocked());
        Assert.False(actualEntity.LockedUntil.IsLocked());
        Assert.Equal(expectedEntity.Id, actualEntity.Id);
        Assert.Equal(expectedEntity.TestParameterString, actualEntity.TestParameterString);
    }
    
    [Fact]
    public async Task BeginCriticalSection_Concurrency()
    {
        //Arrange
        const int noOfTasks = 10;
        var expectedEntity = new HasTimeLockEntity
        {
            TestParameterString = Guid.NewGuid().ToString()
        };
        await Context.HasTimeLockEntities.AddAsync(expectedEntity);
        await Context.SaveChangesAsync();
        
        //Act
        var databaseFactory = new DatabaseContextFactory();
        var tasks = Enumerable.Range(0, noOfTasks).Select(taskIndex =>
        {
            var dbContext = databaseFactory.CreateDbContext([]);
            return LockEntityAndChangeParam(dbContext, expectedEntity, taskIndex);
        });
        
        var response = (await Task.WhenAll(tasks)).ToList();
        var taskIndexThatAcquiredLock = response.FindIndex(lockWasAcquired => lockWasAcquired);
        logger.WriteLine($"Lock acquired for task on index {taskIndexThatAcquiredLock}");
        
        //Assertions
        //Assert only a single task claimed the lock
        Assert.Single(response, lockWasAcquired =>lockWasAcquired);
        
        //Assert that all other tasks did not claim the lock
        Assert.Equal(noOfTasks-1, response.Count(lockAcquired => lockAcquired == false));
        
        var actualEntity = await databaseFactory.CreateDbContext([]).HasTimeLockEntities.SingleAsync(x=>x.Id == expectedEntity.Id);
        Assert.NotNull(actualEntity.LockedUntil);
        Assert.True(actualEntity.LockedUntil?.IsLocked());
        Assert.False(actualEntity.LockedUntil?.IsNotLocked());
        Assert.Equal(expectedEntity.Id, actualEntity.Id);
        Assert.Equal(expectedEntity.TestParameterString, taskIndexThatAcquiredLock.ToString());
    }

    private async Task<bool> LockEntityAndChangeParam(DatabaseContext db,
        HasTimeLockEntity expectedEntity, int testParam)
    {
        //introduce a random delay; this is to randomize the task that will succeed in acquiring the lock,
        //thus moving a bit closer to real life scenarios
        await Task.Delay(new Random().Next(0, 20));
        var criticalSectionService = new CriticalSectionService<DatabaseContext>(db);
        
        var entityToLock = await db.HasTimeLockEntities.SingleAsync(x=>x.Id == expectedEntity.Id);

        var opts = (CriticalSectionEfOptions x) =>
        {
            x.AutoUnlockOnCriticalSectionExit = false;
            x.OnConcurrencyResolutionHandle = _ =>
            {
                logger.WriteLine($"Concurrency handled for task: {testParam}");
                return Task.CompletedTask;
            };
        };
        
        await using (var criticalSection = await criticalSectionService.BeginCriticalSectionAndCommitAsync(entityToLock, TimeSpan.FromMinutes(1), opts))
        {
            if (criticalSection.IsLockAcquired == false)
            {
                return false;
            }

            logger.WriteLine($"Writing value {testParam}. {DateTimeOffset.Now:o}");
            expectedEntity.TestParameterString = testParam.ToString();

            await db.SaveChangesAsync();
        }

        return true;
    }
}