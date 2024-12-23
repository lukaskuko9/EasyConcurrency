using EasyConcurrency.EntityFramework.LockableEntity;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Sample_02_Locking;

public static class Program
{
    private static readonly DatabaseContextFactory DbContextFactory = new();
    private static readonly Guid InsertedWithNoLockUuid1 = Guid.NewGuid();
    private static readonly Guid InsertedWithNoLockUuid2 = Guid.NewGuid();
    
    public static async Task Main(string[] args)
    {
        await DbContextFactory.CreateCleanDatabase(args);
        var dbContext = DbContextFactory.CreateDbContext(args);
        
        //add new entity with no lock
        dbContext.Add(new SampleEntity
        {
            MyUuid = InsertedWithNoLockUuid1,
            IsProcessed = false
        });
        
        //add new entity with no lock
        var noLockEntity = new SampleEntity
        {
            MyUuid = InsertedWithNoLockUuid2,
            IsProcessed = false
        };
        dbContext.Add(noLockEntity);
        
        //add new entity with a lock so no process is able to write changes other than this one
        dbContext.Add(new SampleEntity
        {
            MyUuid = Guid.NewGuid(),
            IsProcessed = false,
            LockedUntil = DateTimeOffset.UtcNow.AddMinutes(5)
        });

        await dbContext.SaveChangesAsync();
        
        // 1) db call to fetch the entity and then lock
        var locked1 = await dbContext.Set<SampleEntity>()
            .Where(sampleEntity => sampleEntity.MyUuid == InsertedWithNoLockUuid1)
            .SingleAsync();
        
        locked1.SetLock(TimeSpan.FromMinutes(5));
        
        //if this entity was locked before this process could lock it, this would throw DbUpdateConcurrencyException
        await dbContext.SaveChangesAsync();
        
        // 2) a single db call to execute update on the entity without DbUpdateConcurrencyException being thrown on concurrency
        var updatedRows = await dbContext.Set<SampleEntity>()
            .Where(sampleEntity => sampleEntity.MyUuid == InsertedWithNoLockUuid2)
            .WhereIsNotLocked() //it is important to only do this for not locked entity, otherwise we won't be concurrency-safe
            .ExecuteUpdateAsync(x => x.SetProperty(
                sampleEntity => sampleEntity.LockedUntil, DateTimeOffset.UtcNow.AddMinutes(5))
            );
        if (updatedRows == 1)
        {
            //we have managed to lock 
        }
        else
        {
            //we have not managed to lock it, it was already locked by other process
        }
    }
}