using EasyConcurrency.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EasyConcurrency.EntityFramework.ConcurrentRepository;

public abstract class ConcurrentRepository<TContext>(TContext databaseContext) : IConcurrentRepository
    where TContext: DbContext
{
    #region Locks
    public async Task<TLockableEntity?> LockAndSaveAsync<TLockableEntity>(TLockableEntity? dbEntity, TimeSpan lockTimeSpan, 
        Action<IReadOnlyList<EntityEntry>>? actionOnConcurrency = null, CancellationToken cancellationToken = default) where TLockableEntity : class, ILockableEntity
    {
        if (dbEntity == null)
            return null;
        
        var locked = await LockAndSaveInnerAsync([dbEntity], lockTimeSpan, actionOnConcurrency, cancellationToken);
        return locked.FirstOrDefault();
    }

    public async Task<List<TLockableEntity>> LockAndSaveAsync<TLockableEntity>(
        IEnumerable<TLockableEntity> dbEntity, TimeSpan lockTimeSpan, Action<IReadOnlyList<EntityEntry>>? actionOnConcurrency = null,
        CancellationToken cancellationToken = default) where TLockableEntity : ILockableEntity
    {
        return await LockAndSaveInnerAsync(dbEntity, lockTimeSpan, actionOnConcurrency, cancellationToken);
    }

    private async Task<List<TLockableEntity>> LockAndSaveInnerAsync<TLockableEntity>(IEnumerable<TLockableEntity> entitiesToLock, TimeSpan lockTimeSpan, 
        Action<IReadOnlyList<EntityEntry>>? action, CancellationToken cancellationToken = default) where TLockableEntity : ILockableEntity
    {
        try
        {
            var lockedEntities = (
                    from entity in entitiesToLock
                    let isLocked = entity.SetLock(lockTimeSpan)
                    where isLocked
                    select entity
                )
                .ToList();
            if (lockedEntities.Count == 0)
                return lockedEntities;
            
            await databaseContext.SaveChangesAsync(cancellationToken);
            return lockedEntities;
        }
        catch (DbUpdateConcurrencyException concurrencyException)
        {
            action?.Invoke(concurrencyException.Entries);

            //no entities locked, return empty collection
            return [];
        }
    }
    
    #endregion
    public async Task<bool> InsertAndSaveAsync<TLockableEntity>(TLockableEntity entity, CancellationToken token = default) where TLockableEntity: class, ILockableEntity
    {
        try
        {
            await databaseContext.Set<TLockableEntity>().AddAsync(entity, token);
            await databaseContext.SaveChangesAsync(token);
            return true;
        }
        catch (DbUpdateException exception) when (DbExceptionErrorCodes.IsUniqueConstraintViolation(exception))
        {
            return false;
        }
    }
}