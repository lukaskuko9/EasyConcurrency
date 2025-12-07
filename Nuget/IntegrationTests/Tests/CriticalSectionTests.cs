using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.EntityFramework.Entities;
using IntegrationTests.Database;
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
            TestParameterString = null
        };
        await Context.MyLockableEntities.AddAsync(expectedEntity);
        await Context.SaveChangesAsync();
        
        //Act
        var databaseFactory = new DatabaseContextFactory();
        var tasks = Enumerable.Range(0, 10).Select(changeParamToValue => LockEntityAndChangeParam(databaseFactory, expectedEntity, changeParamToValue.ToString()));
        var response = (await Task.WhenAll(tasks)).ToList();
        var numberShouldBe = response.FindIndex(lockWasAcquired => lockWasAcquired);
        logger.WriteLine($"Lock acquired for task on index {numberShouldBe}");
        
        //Assert
        var actualEntity = await Context.MyLockableEntities.FindAsync(expectedEntity.Id);
        Assert.NotNull(actualEntity?.LockedUntil);
        Assert.Single(response, lockWasAcquired =>lockWasAcquired);
        Assert.Equal(expectedEntity.Id, actualEntity.Id);
        Assert.Equal(expectedEntity.TestParameterString, numberShouldBe.ToString());
    }

    private async Task<bool> LockEntityAndChangeParam(DatabaseContextFactory databaseFactory,
        MyLockableEntity expectedEntity, string testParam)
    {
        await Task.Delay(new Random().Next(10, 200));
        var criticalSectionService = new CriticalSectionService<DatabaseContext>(databaseFactory.CreateDbContext([]));
        await using var criticalSection = await criticalSectionService.BeginCriticalSectionAndCommitAsync(expectedEntity, TimeSpan.FromMinutes(1), false);
        if (criticalSection.LockAcquired == false)
        {
            return false;
        }

        logger.WriteLine($"Writing value {testParam}");
        expectedEntity.TestParameterString = testParam;
        await Context.SaveChangesAsync();
        return true;
    }
}