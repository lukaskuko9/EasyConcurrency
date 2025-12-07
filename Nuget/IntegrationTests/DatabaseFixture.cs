using EasyConcurrency.IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.IntegrationTests;

public class DatabaseFixture : IDisposable
{
    protected readonly DatabaseContext Context;

    protected DatabaseFixture()
    {
        var factory = new DatabaseContextFactory();
        Context = factory.CreateDbContext([]);
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.MyDbEntities.ExecuteDelete();
    }

    
}