using Microsoft.EntityFrameworkCore;
using UnitTests.Database;
using Xunit;
using Xunit.Sdk;

namespace UnitTests;

public class LockedUntilTests : TestClass
{
    private static readonly DatabaseContext Context;

    static LockedUntilTests()
    {
        var connectionString = "Data Source=.;Initial Catalog=EFConcurrencyTests;Integrated Security=True;TrustServerCertificate=True";
        var factory = new DatabaseContextFactory();
        Context = factory.CreateDbContext([connectionString]);
        Context.Database.EnsureDeleted();
        Context.Database.Migrate();
    }

    [Fact]
    public async Task LockedUntilTranslatesCorrectly()
    {
        var newEntityNotLocked = new MyDbEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = null
        };
        
        var lockedUntil = DateTimeOffset.Now.AddMinutes(10);
        var newEntityLocked = new MyDbEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = lockedUntil
        };
        
        await Context.MyDbEntities.AddAsync(newEntityNotLocked);
        await Context.MyDbEntities.AddAsync(newEntityLocked);
        await Context.SaveChangesAsync();
        
        var dbEntityNotLocked = await Context.MyDbEntities.SingleAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityNotLocked.MyUniqueKey);
        Assert.True(dbEntityNotLocked.IsNotLocked());
        
        var dbEntityLocked = await Context.MyDbEntities.SingleAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityLocked.MyUniqueKey);
        Assert.False(dbEntityLocked.IsNotLocked());
        Assert.Equal(dbEntityLocked.LockedUntil, lockedUntil);
    }
}