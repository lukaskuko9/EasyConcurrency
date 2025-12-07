using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.EntityFramework.Entities;
using EasyConcurrency.IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;
using Stubs;
using Xunit;
using Xunit.Abstractions;

namespace EasyConcurrency.IntegrationTests.Tests;

[Collection(DatabaseCollection.CollectionName)]
public class CriticalSectionTests(ITestOutputHelper logger) : DatabaseFixture
{
    [Fact]
    public async Task BeginCriticalSection_NoConcurrency()
    {
        //Arrange
        var criticalSectionService = new CriticalSectionService<DatabaseContext>(Context);
        var expectedEntity = new MyLockableEntity
        {
            TestParameterGuid = Guid.NewGuid(),
            TestParameterString = "TestValue"
        };
        await Context.MyLockableEntities.AddAsync(expectedEntity);
        await Context.SaveChangesAsync();
        
        //Act
        await using (var criticalSection = await criticalSectionService.BeginCriticalSectionAndCommitAsync(expectedEntity, TimeSpan.FromMinutes(1)))
        {
            if (criticalSection.IsLockAcquired && expectedEntity.LockedUntil.IsNotLocked())
            {
                throw new ApplicationException("The entity is NOT locked");
            }
            
            var c = await Context.MyLockableEntities.SingleAsync(x=>x.Id == expectedEntity.Id);
            Assert.NotNull(c?.LockedUntil);
        }
        
        //Assert
        var actualEntity = await Context.MyLockableEntities.SingleAsync(x=>x.Id == expectedEntity.Id);
        Assert.Null(actualEntity?.LockedUntil);
        Assert.Equal(expectedEntity.Id, actualEntity?.Id);
        Assert.Equal(expectedEntity.TestParameterGuid, actualEntity?.TestParameterGuid);
        Assert.Equal(expectedEntity.TestParameterString, actualEntity?.TestParameterString);
    }
    
    [Fact]
    public async Task BeginCriticalSection_Concurrency()
    {
        //Arrange
        const int noOfTasks = 10;
        var expectedEntity = new MyLockableEntity
        {
            TestParameterGuid = Guid.NewGuid()
        };
        await Context.MyLockableEntities.AddAsync(expectedEntity);
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
        
        //Assert
        Assert.Single(response, lockWasAcquired =>lockWasAcquired);
        Assert.Equal(noOfTasks-1, response.Count(lockAcquired => lockAcquired == false));
        
        var actualEntity = await databaseFactory.CreateDbContext([]).MyLockableEntities.SingleAsync(x=>x.Id == expectedEntity.Id);
        Assert.NotNull(actualEntity.LockedUntil);
        Assert.Equal(expectedEntity.Id, actualEntity.Id);
        Assert.Equal(expectedEntity.TestParameterString, taskIndexThatAcquiredLock.ToString());
    }

    private async Task<bool> LockEntityAndChangeParam(DatabaseContext db,
        MyLockableEntity expectedEntity, int testParam)
    {
        //introduce a random delay; this is to randomize the task that will succeed in acquiring the lock,
        //thus moving a bit closer to real life scenarios
        await Task.Delay(new Random().Next(0, 20));
        var criticalSectionService = new CriticalSectionService<DatabaseContext>(db);
        
        var entityToLock = await db.MyLockableEntities.SingleAsync(x=>x.Id == expectedEntity.Id);

        var opts = (CriticalSectionOptions x) =>
        {
            x.AutoUnlockOnCriticalSectionExit = false;
            x.OnConcurrencyResolutionHandle = _ =>
            {
                logger.WriteLine($"Concurrency handled for task: {testParam}");
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