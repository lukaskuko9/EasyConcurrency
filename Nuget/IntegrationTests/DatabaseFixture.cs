using EasyConcurrency.IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.IntegrationTests;

public class DatabaseFixture : IDisposable, IAsyncDisposable
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
        Context.Dispose();
    }
    
    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
    }
}