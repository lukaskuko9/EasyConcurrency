using EasyConcurrency.EntityFramework.LockableEntity;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Sample_01_Fetching;

public static class Program
{
    private static readonly DatabaseContextFactory DbContextFactory = new();

    public static async Task Main(string[] args)
    {
        await DbContextFactory.CreateCleanDatabase(args);

        //add entity we will be working with
        var dbContext = DbContextFactory.CreateDbContext(args);
        
        /* Generates query:
          SELECT [s].[Id], [s].[IsProcessed], [s].[LockedUntil], [s].[MyUuid], [s].[Version]
          FROM [SampleEntities] AS [s]
          WHERE [s].[LockedUntil] IS NULL OR CAST([s].[LockedUntil] AS datetimeoffset) < @__now_0
        */
        await dbContext
            .Set<SampleEntity>()
            .WhereIsNotLocked()
            .ToListAsync();

        /* Generates Query:
          SELECT [s].[MyUuid]
          FROM [SampleEntities] AS [s]
          WHERE [s].[IsProcessed] = CAST(1 AS bit) AND ([s].[LockedUntil] IS NULL OR CAST([s].[LockedUntil] AS datetimeoffset) < @__now_0)
          ORDER BY [s].[Id] DESC
          OFFSET @__p_1 ROWS FETCH NEXT @__p_2 ROWS ONLY
         */
        await dbContext
            .Set<SampleEntity>()
            .Where(sampleEntity => sampleEntity.IsProcessed)
            .WhereIsNotLocked(DateTimeOffset.Now)
            .OrderByDescending(sampleEntity => sampleEntity.Id)
            .Select(sampleEntity => sampleEntity.MyUuid)
            .Skip(5)
            .Take(10)
            .ToArrayAsync();
    }
}