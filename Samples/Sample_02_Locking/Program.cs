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
        var dbContext = await PrepareDatabaseWithData(args);

        // 1) db call to fetch the entity and then lock
        await BasicLock(dbContext);

        // 2) a single db call to execute update on the entity without DbUpdateConcurrencyException being thrown on concurrency
        await LockWithoutConcurrencyException(dbContext);
    }

    private static async Task BasicLock(DatabaseContext dbContext)
    {
        var entity = await dbContext.Set<SampleEntity>()
            .WhereIsNotLocked()
            .Where(sampleEntity => sampleEntity.MyUuid == InsertedWithNoLockUuid1)
            .SingleOrDefaultAsync();

        if (entity is null)
        {
            //we know it exists because we have just created it, so it has to be locked
            //this won't happen in this sample, but this is where you deal with non-existing or already locked entity
            throw new Exception("Entity is locked!");
        }

        entity.SetLock(TimeSpan.FromMinutes(5));

        try
        {
            //if this entity was locked after we fetched the entity before this process could lock it,
            //SaveChangesAsync would throw DbUpdateConcurrencyException
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            //resolve concurrency exception here
        }
    }

    private static async Task LockWithoutConcurrencyException(DatabaseContext dbContext)
    {
        var updatedRows = await dbContext.Set<SampleEntity>()
            .Where(sampleEntity => sampleEntity.MyUuid == InsertedWithNoLockUuid2)
            .WhereIsNotLocked() //it is important to only do this for not locked entity, otherwise we won't be concurrency-safe
            .ExecuteUpdateAsync(x => x.SetProperty(
                    sampleEntity => sampleEntity.LockedUntil,
                    DateTimeOffset.UtcNow.AddMinutes(5) //lock for 5 minutes
                )
            );

        if (updatedRows == 1) //MyUuid prop is unique, so we know there can be only 0 or 1 rows updated
        {
            //we have managed to lock 
        }
        else
        {
            //we have not managed to lock it, it was already locked when db command executed or entity with that UUID doesn't exist
        }
    }

    private static async Task<DatabaseContext> PrepareDatabaseWithData(string[] args)
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
        dbContext.Add(new SampleEntity
        {
            MyUuid = InsertedWithNoLockUuid2,
            IsProcessed = false
        });

        //add new entity with a lock so no process is able to write changes other than this one
        dbContext.Add(new SampleEntity
        {
            MyUuid = Guid.NewGuid(),
            IsProcessed = false,
            LockedUntil = DateTimeOffset.UtcNow.AddMinutes(5)
        });

        await dbContext.SaveChangesAsync();
        return dbContext;
    }
}