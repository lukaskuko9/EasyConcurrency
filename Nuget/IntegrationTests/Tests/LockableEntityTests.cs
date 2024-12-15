using EasyConcurrency.EntityFramework.LockableEntity;
using IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;
using Stubs;
using Xunit;

namespace IntegrationTests.Tests;

[Collection(DatabaseCollection.CollectionName)]
public class LockableEntityTests : DatabaseFixture
{
    [Fact]
    public async Task ConcurrencyTokenTakesEffect()
    {
        var entity = new MyLockableEntity
        {
            LockedUntil = null
        };

        await Context.MyLockableEntities.AddAsync(entity);
        await Context.SaveChangesAsync();

        var databaseFactory = new DatabaseContextFactory();
        var tasks = Enumerable.Range(0, 10).Select(_ => GetAndLockEntity(databaseFactory.CreateDbContext([])));
        var completedTasks = await Task.WhenAll(tasks);
        Assert.NotEmpty(completedTasks);

        var lockedEntities = completedTasks
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();
        
        //verify exactly 1 entity is locked
        var lockedEntity = Assert.Single(lockedEntities);
        Assert.False(lockedEntity.IsNotLocked());
    }

    private static async Task<MyLockableEntity?> GetAndLockEntity(DatabaseContext dbContext)
    {
        try
        {
            var entityToLock = await dbContext.MyLockableEntities.WhereIsNotLocked().SingleOrDefaultAsync();
            if (entityToLock is null)
                return null;

            entityToLock.SetLock(5);
            await dbContext.SaveChangesAsync();
            return entityToLock;
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
    }
}
