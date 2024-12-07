using EasyConcurrency.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EasyConcurrency.EntityFramework.ConcurrentRepository;

public abstract class ConcurrentRepository<TContext>(TContext databaseContext) : IConcurrentRepository
    where TContext: DbContext
{
    #region Locks
    public async Task<TConcurrentEntity?> LockAndSaveAsync<TConcurrentEntity>(TConcurrentEntity? dbEntity, TimeSpan lockTimeSpan, 
        Action<IReadOnlyList<EntityEntry>>? actionOnConcurrency = null, CancellationToken cancellationToken = default) where TConcurrentEntity : class, ILockableEntity
    {
        if (dbEntity == null)
            return null;
        
        var locked = await LockAndSaveInnerAsync([dbEntity], lockTimeSpan, actionOnConcurrency, cancellationToken);
        return locked.FirstOrDefault();
    }

    public async Task<List<TConcurrentEntity>> LockAndSaveAsync<TConcurrentEntity>(
        IEnumerable<TConcurrentEntity> dbEntity, TimeSpan lockTimeSpan, Action<IReadOnlyList<EntityEntry>>? actionOnConcurrency = null,
        CancellationToken cancellationToken = default) where TConcurrentEntity : ILockableEntity
    {
        return await LockAndSaveInnerAsync(dbEntity, lockTimeSpan, actionOnConcurrency, cancellationToken);
    }

    private async Task<List<TConcurrentEntity>> LockAndSaveInnerAsync<TConcurrentEntity>(IEnumerable<TConcurrentEntity> entitiesToLock, TimeSpan lockTimeSpan, 
        Action<IReadOnlyList<EntityEntry>>? action, CancellationToken cancellationToken = default) where TConcurrentEntity : ILockableEntity
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
    public async Task<bool> InsertAndSaveAsync<TConcurrentEntity>(TConcurrentEntity entity, CancellationToken token = default) where TConcurrentEntity: class, ILockableEntity
    {
        try
        {
            await databaseContext.Set<TConcurrentEntity>().AddAsync(entity, token);
            await databaseContext.SaveChangesAsync(token);
            return true;
        }
        catch (DbUpdateException exception) when (DbExceptionErrorCodes.IsUniqueConstraintViolation(exception))
        {
            return false;
        }
    }
}