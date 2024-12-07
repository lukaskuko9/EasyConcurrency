using IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests;

public class DatabaseFixture : IDisposable
{
    protected readonly DatabaseContext Context;
    protected readonly MyConcurrentRepository Repository;

    protected DatabaseFixture()
    {
        var connectionString = DatabaseContext.GetConnectionString();
        var factory = new DatabaseContextFactory();
        Context = factory.CreateDbContext([connectionString]);
        Context.Database.EnsureDeleted();
        Context.Database.Migrate();
        
        Repository = new MyConcurrentRepository(Context);
    }

    public void Dispose()
    {
        Context.MyDbEntities.ExecuteDelete();
    }

    
}