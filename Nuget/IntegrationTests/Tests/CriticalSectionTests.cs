using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.EntityFramework.Entities;
using IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;
using Stubs;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

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
            if (criticalSection.LockAcquired && expectedEntity.LockedUntil.IsNotLocked())
            {
                throw new ApplicationException("The entity is NOT locked");
            }
            
            var c = await Context.MyLockableEntities.FindAsync(expectedEntity.Id);
            Assert.NotNull(c?.LockedUntil);
        }
        
        //Assert
        var actualEntity = await Context.MyLockableEntities.FindAsync(expectedEntity.Id);
        Assert.Null(actualEntity?.LockedUntil);
        Assert.Equal(expectedEntity.Id, actualEntity?.Id);
        Assert.Equal(expectedEntity.TestParameterGuid, actualEntity?.TestParameterGuid);
        Assert.Equal(expectedEntity.TestParameterString, actualEntity?.TestParameterString);
    }
    
    [Fact]
    public async Task BeginCriticalSection_Concurrency()
    {
        //Arrange
        var expectedEntity = new MyLockableEntity
        {
            TestParameterGuid = Guid.NewGuid()
        };
        await Context.MyLockableEntities.AddAsync(expectedEntity);
        await Context.SaveChangesAsync();
        
        //Act
        var databaseFactory = new DatabaseContextFactory();
        var tasks = Enumerable.Range(0, 10).Select(changeParamToValue =>
        {
            var db = databaseFactory.CreateDbContext([]);
            return LockEntityAndChangeParam(db, expectedEntity, changeParamToValue.ToString());
        });
        var response = (await Task.WhenAll(tasks)).ToList();
        var numberShouldBe = response.FindIndex(lockWasAcquired => lockWasAcquired);
        logger.WriteLine($"Lock acquired for task on index {numberShouldBe}");
        
        //Assert
        var actualEntity = await databaseFactory.CreateDbContext([]).MyLockableEntities.SingleAsync(x=>x.Id == expectedEntity.Id);
        Assert.NotNull(actualEntity.LockedUntil);
        Assert.Single(response, lockWasAcquired =>lockWasAcquired);
        Assert.Equal(expectedEntity.Id, actualEntity.Id);
        Assert.Equal(expectedEntity.TestParameterString, numberShouldBe.ToString());
    }

    private async Task<bool> LockEntityAndChangeParam(DatabaseContext db,
        MyLockableEntity expectedEntity, string testParam)
    {
        await Task.Delay(new Random().Next(10, 20));
        var criticalSectionService = new CriticalSectionService<DatabaseContext>(db);
        
       
        var entityToLock = await db.MyLockableEntities.SingleAsync(x=>x.Id == expectedEntity.Id);

        var opts = (CriticalSectionOptions x) =>
        {
            x.AutoUnlockOnCriticalSectionExit = false;
        };
        
        await using (var criticalSection = await criticalSectionService.BeginCriticalSectionAndCommitAsync(entityToLock, TimeSpan.FromMinutes(1), opts))
        {
            if (criticalSection.LockAcquired == false)
            {
                return false;
            }

            logger.WriteLine($"Writing value {testParam}. {DateTimeOffset.Now:o}");
            expectedEntity.TestParameterString = testParam;

            await db.SaveChangesAsync();
        }

        return true;
    }
}