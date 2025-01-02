using EasyConcurrency.Abstractions.TimeLockNamespace;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Sample_03_Unlocking;

public static class Program
{
    private static readonly DatabaseContextFactory DbContextFactory = new();
    private static readonly Guid UniqueIdentifier = Guid.NewGuid();

    public static async Task Main(string[] args)
    {
        await DbContextFactory.CreateCleanDatabase(args);
        var dbContext = DbContextFactory.CreateDbContext(args);

        var entity = new SampleEntity
        {
            MyUuid = UniqueIdentifier,
            IsProcessed = false,
            LockedUntil = DateTimeOffset.UtcNow.AddMinutes(5) //create with lock
        };
        
        //add new entity with no lock
        dbContext.Add(entity);
        await dbContext.SaveChangesAsync();
        
        //1) unlock with instance
        entity.Unlock();
        await dbContext.SaveChangesAsync();
        
        //2) unlock with Id
        await UnlockWithId(dbContext);
    }

    private static async Task UnlockWithId(DatabaseContext dbContext)
    {
        //we don't have to worry about concurrency when unlocking
        //if it is respected properly, only what locked it will be able to unlock it
        await dbContext.Set<SampleEntity>()
            .Where(sampleEntity => sampleEntity.MyUuid == UniqueIdentifier)
            .ExecuteUpdateAsync(x => x.SetProperty
                (
                    sampleEntity => sampleEntity.LockedUntil,
                    (TimeLock?)null
                )
            );
    }
}