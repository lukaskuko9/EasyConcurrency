using IntegrationTests.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using Stubs;
using Xunit;

namespace IntegrationTests.Tests;

[Collection(DatabaseCollection.CollectionName)]
public class ConcurrentRepositoryTests : DatabaseFixture
{
    [Fact]
    public async Task ExceptionOnUniqueViolationIsHandled()
    {
        var uniqueKey = Guid.NewGuid();
        var lockedUntil = DateTimeOffset.UtcNow.AddMinutes(10);
        var newEntityLocked = new MyDbVersioningEntity
        {
            MyUniqueKey = uniqueKey,
            LockedUntil = lockedUntil
        };

        var inserted = await Repository.InsertAndSaveAsync(newEntityLocked);
        Assert.True(inserted);
        
        var newEntityLocked2 = new MyDbVersioningEntity
        {
            MyUniqueKey = uniqueKey,
            LockedUntil = lockedUntil
        };

        var insertedEntities = await Repository.InsertAndSaveAsync([newEntityLocked2]);
        Assert.Empty(insertedEntities);
    }

    [Fact]
    public async Task NotLockedEntityCanBeLocked()
    {
        var uniqueKey = Guid.NewGuid();
        var newEntityLocked = new MyDbVersioningEntity
        {
            MyUniqueKey = uniqueKey,
            LockedUntil = null
        };
        
        var inserted = await Repository.InsertAndSaveAsync(newEntityLocked);
        Assert.True(inserted);
        
        var dbEntity = await Repository.LockAndSaveAsync(newEntityLocked, TimeSpan.FromMinutes(5));
        Assert.NotNull(dbEntity);
        
        Assert.True(dbEntity.MyUniqueKey == uniqueKey);
        Assert.False(dbEntity.IsNotLocked());
    }

    [Fact]
    public async Task AllEntitiesCanBeInserted()
    {
        var newEntities = Enumerable
            .Range(0, 10)
            .Select(_ => new MyDbVersioningEntity
                {
                    MyUniqueKey = Guid.NewGuid(),
                    LockedUntil = null
                }
            )
            .ToList();

        var insertedEntities = await Repository.InsertAndSaveAsync(newEntities);
        Assert.NotNull(insertedEntities);
        Assert.True(insertedEntities.SequenceEqual(newEntities));
    }

    [Fact]
    public async Task ActionIsInvokedOnLockAndSave()
    {
        var uniqueKey = Guid.NewGuid();
        var newEntityLocked = new MyDbVersioningEntity
        {
            MyUniqueKey = uniqueKey,
            LockedUntil = null
        };
        var actionWasCalled = false;
        
        //mock our way through this one to simulate concurrency update exception
        var mockedDbContext = new Mock<DatabaseContext>();
        mockedDbContext
            .Setup(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new DbUpdateConcurrencyException());
        
        var mockedRepository = new MyConcurrentRepository(mockedDbContext.Object);
        await mockedRepository.LockAndSaveAsync(newEntityLocked, TimeSpan.FromMinutes(5), 
            _ => actionWasCalled = true
            );
        
        Assert.True(actionWasCalled);
    }
}