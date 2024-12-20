using EasyConcurrency.EntityFramework.LockableEntity;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Sample_01_Setup;

public static class Program
{
    private static readonly DatabaseContextFactory DbContextFactory = new();
    private static readonly Guid ConcurrentEntityUuid = Guid.NewGuid();
    
    public static async Task Main(string[] args)
    {
        await CreateCleanDatabase(args, DbContextFactory);

        var tasks = Enumerable.Range(1, 3).Select(i => ParallelTask(i, args));
        await Task.WhenAll(tasks);
    }

    private static async Task ParallelTask(int i, string[] args)
    {
        while (true)
        {
            var dbContext = CreateDatabaseContext(args, DbContextFactory);
            var entity = await dbContext.SampleEntities
                .WhereIsNotLocked()
                .SingleOrDefaultAsync(x => x.MyUuid == ConcurrentEntityUuid);
            if (entity is null)
            {
                var randomNumber = new Random().Next(100, 300);
                await Task.Delay(randomNumber);
                Console.WriteLine(
                    $"Task {i} was trying to get entity but it is currently locked. Trying again in {randomNumber} milliseconds.");
                continue;
            }

            try
            {
                //lock entity
                entity.SetLock(5);

                //save lock to database
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Task {i} successfully locked entity");

                //simulate processing entity
                var randomDelay = new Random().Next(100, 300);
                await Task.Delay(randomDelay);

                //unlock entity
                entity.Unlock();

                //save changes to database
                await dbContext.SaveChangesAsync();
                
                Console.WriteLine($"Task {i} unlocked entity for other tasks");

                //delay again so that there is time for other tasks to lock it
                randomDelay = new Random().Next(300, 500);
                await Task.Delay(randomDelay);

            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine($"Task {i} failed to lock entity. Exception was handled");
            }
        }
    }

    private static async Task CreateCleanDatabase(string[] args, DatabaseContextFactory dbContextFactory)
    {
        await using var dbContext = CreateDatabaseContext(args, dbContextFactory);

        //make sure we delete anything we previously stored so we can start from fresh
        await dbContext.Database.EnsureDeletedAsync();
        
        //ensure that database is created again
        await dbContext.Database.EnsureCreatedAsync();

        //add entity we will be working with
        await dbContext.SampleEntities.AddAsync(new SampleEntity
        {
            MyUuid = ConcurrentEntityUuid
        });
        await dbContext.SaveChangesAsync();
    }

    private static DatabaseContext CreateDatabaseContext(string[] args, DatabaseContextFactory dbContextFactory)
    {
        //create database context; for simplicity we create it from factory
        return dbContextFactory.CreateDbContext(args);
    }
}