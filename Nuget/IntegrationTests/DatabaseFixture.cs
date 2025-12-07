using EasyConcurrency.IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.IntegrationTests;

public class DatabaseFixture : IDisposable
{
    protected readonly DatabaseContext Context;
    protected readonly MyConcurrentRepository Repository;

    protected DatabaseFixture()
    {
        var factory = new DatabaseContextFactory();
        Context = factory.CreateDbContext([]);
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        Repository = new MyConcurrentRepository(Context);
    }

    public void Dispose()
    {
        Context.MyDbEntities.ExecuteDelete();
    }

    
}