using EasyConcurrency.EntityFramework.LockableEntity;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FullSample_01_Pessimistic_Processing;

public static class Program
{
    private static readonly DatabaseContextFactory DbContextFactory = new();
    private static readonly Guid ConcurrentEntityUuid = Guid.NewGuid();

    public static async Task Main(string[] args)
    {
        await DbContextFactory.CreateCleanDatabase(args);

        //add entity we will be working with
        var dbContext = DbContextFactory.CreateDbContext(args);
        await dbContext.AddAsync(new SampleEntity
        {
            MyUuid = ConcurrentEntityUuid
        });
        await dbContext.SaveChangesAsync();

        //run 3 tasks in parallel
        const int numberOfConcurrentTasks = 3;
        var tasks = Enumerable.Range(1, numberOfConcurrentTasks).Select(i => ProcessEntity(i, args));
        await Task.WhenAll(tasks);
    }

    private static async Task ProcessEntity(int i, string[] args)
    {
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("Cancel event triggered");
            cts.Cancel();
            eventArgs.Cancel = true;
        };

        while (true)
        {
            var dbContext = DbContextFactory.CreateDatabaseContext(args);

            //get entity only if not locked; if entity is locked this will return null;
            var entity = await dbContext.SampleEntities
                .WhereIsNotLocked()
                .SingleOrDefaultAsync(sampleEntity => sampleEntity.MyUuid == ConcurrentEntityUuid,
                    cancellationToken: cts.Token);

            if (entity is null)
            {
                //entity is null, we cannot process it
                const int waitTime = 200;
                await Task.Delay(waitTime, cts.Token);
                Console.WriteLine(
                    $"Task {i} was trying to get entity but it is currently locked. Trying again in {waitTime} milliseconds.");
                continue;
            }

            try
            {
                //lock entity
                entity.SetLock(TimeSpan.FromMinutes(5));

                //save lock to database
                await dbContext.SaveChangesAsync(cts.Token);
                Console.WriteLine($"Task {i} successfully locked entity");
            }
            catch (DbUpdateConcurrencyException)
            {
                //failed to lock entity, entity was locked by other task after we got the entity from database
                Console.WriteLine($"Task {i} failed to lock entity. Exception was handled");
                continue;
            }

            //simulate delay during actual processing of an entity
            Console.WriteLine($"Task {i} is processing entity");
            var randomDelay = new Random().Next(500, 1000);
            await Task.Delay(randomDelay, cts.Token);

            //unlock entity and save to database
            entity.Unlock();
            await dbContext.SaveChangesAsync(cts.Token);
            Console.WriteLine($"Task {i} unlocked entity for other tasks");

            //wait 1 second so other tasks can have chance to claim the entity as well
            await Task.Delay(1000, cts.Token);
        }
    }
}