using EasyConcurrency.EntityFramework;
using IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests;

public class LockedUntilTests
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
        
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
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

    [Fact]
    public async Task LockIsRespected()
    {
        var newEntityNotLocked = new MyDbEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = null
        };
        
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        var newEntityLocked = new MyDbEntity
        {
            MyUniqueKey = Guid.NewGuid(),
            LockedUntil = lockedUntil
        };
        
        await Context.MyDbEntities.AddAsync(newEntityNotLocked);
        await Context.MyDbEntities.AddAsync(newEntityLocked);
        await Context.SaveChangesAsync();
        
        var dbEntityNotLocked = await Context.MyDbEntities
            .WhereIsNotLocked()
            .SingleAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityNotLocked.MyUniqueKey);
        
        var dbEntityLockExpired = await Context.MyDbEntities
            .WhereIsNotLocked(DateTimeOffset.UtcNow.AddMinutes(30))
            .SingleOrDefaultAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityLocked.MyUniqueKey);
        
        var dbEntityLocked = await Context.MyDbEntities
            .WhereIsNotLocked()
            .SingleOrDefaultAsync(myDbEntity => myDbEntity.MyUniqueKey == newEntityLocked.MyUniqueKey);
        
        //not locked entity can be fetched and is not locked
        Assert.True(dbEntityNotLocked.IsNotLocked());

        //locked entity cannot be fetched as it is locked
        Assert.Null(dbEntityLocked);
        
        //when lock has expired, entity that was locked is automatically unlocked now and can be fetched
        Assert.NotNull(dbEntityLockExpired);
        Assert.Equal(dbEntityLockExpired.LockedUntil, lockedUntil);
    }
}