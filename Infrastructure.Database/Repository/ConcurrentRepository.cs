using Infrastructure.Database.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Database.Repository;

public abstract class ConcurrentRepository(DatabaseContext databaseContext)
{
    #region Locks
    protected async Task<T?> LockAndSaveAsync<T>(T? dbEntity, TimeSpan lockTimeSpan, 
        Action<IReadOnlyList<EntityEntry>>? concurrencyAction = null, CancellationToken cancellationToken = default) where T : ConcurrentEntity
    {
        if (dbEntity == null)
            return null;
        
        var locked = await LockAndSaveInnerAsync([dbEntity], lockTimeSpan, concurrencyAction, cancellationToken);
        return locked.FirstOrDefault();
    }

    protected async Task<List<T>> LockAndSaveAsync<T>(IEnumerable<T> dbEntity, TimeSpan lockTimeSpan, 
        Action<IReadOnlyList<EntityEntry>>? concurrencyAction = null, CancellationToken cancellationToken = default) where T : ConcurrentEntity =>
        await LockAndSaveInnerAsync(dbEntity, lockTimeSpan, concurrencyAction, cancellationToken);

    private async Task<List<T>> LockAndSaveInnerAsync<T>(IEnumerable<T> entitiesToLock, TimeSpan lockTimeSpan, 
        Action<IReadOnlyList<EntityEntry>>? action, CancellationToken cancellationToken) where T : ConcurrentEntity
    {
        try
        {
            var lockedEntities = new List<T>();
            foreach (var entity in entitiesToLock)
            {
                var isLocked = entity.SetLock(lockTimeSpan);
                if (isLocked)
                    lockedEntities.Add(entity);
            }

            if (lockedEntities.Count == 0)
                return lockedEntities;
            
            await databaseContext.SaveChangesAsync(cancellationToken);
            return lockedEntities;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (action is not null)
                action.Invoke(ex.Entries);
            else
            {
                //https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#resolving-concurrency-conflicts
                foreach (var entry in ex.Entries)
                {
                    var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                    if (databaseValues is null)
                        continue;

                    // Refresh original values to bypass next concurrency check
                    entry.CurrentValues.SetValues(databaseValues);
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }

            return [];
        }
    }
    
    #endregion
    protected async Task<bool> InsertAsync<T>(T entity, CancellationToken token) where T: ConcurrentEntity
    {
        try
        {
            await databaseContext.Set<T>().AddAsync(entity, token);
            await databaseContext.SaveChangesAsync(token);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException )
        {
            //if unique constraint violation (2601) or unique constraint violation (2627)
            if(sqlException.Number is 2601 or 2627)
                return false;

            throw;
        }
    }
}