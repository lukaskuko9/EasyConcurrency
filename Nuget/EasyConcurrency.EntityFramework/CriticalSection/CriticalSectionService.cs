using EasyConcurrency.Abstractions.CriticalSection;
using EasyConcurrency.Abstractions.TimeLock;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework.CriticalSection;

/// <inheritdoc/>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CriticalSectionService<TDbContext>(TDbContext dbContext, TimeProvider? timeProvider = null) : ICriticalSectionService<CriticalSectionEfOptions> where TDbContext : DbContext
{
    /// <inheritdoc/>
    public virtual async Task<Abstractions.CriticalSection.CriticalSection> BeginCriticalSectionAndCommitAsync<TTimeLock>(IHasTimeLock<TTimeLock> entity, TimeSpan lockForTime,
        Action<CriticalSectionEfOptions>? criticalSectionOptions = null, CancellationToken cancellationToken = default) where TTimeLock : struct, ITimeLock
    {
        var opts = new CriticalSectionEfOptions
        {
            AutoUnlockOnCriticalSectionExit = true
        };
        
        try
        {
            //if entity is already locked we cannot proceed
            if (IsUnlocked(entity, GetCurrentTime()) == false)
                return new Abstractions.CriticalSection.CriticalSection( UnlockFunc, false, opts);
            
            //override options
            criticalSectionOptions?.Invoke(opts);

            //try lock the entity
            entity.LockedUntil = (TTimeLock)TTimeLock.Create(GetCurrentTime().Add(lockForTime));
            await PersistChanges(cancellationToken);
            
            return new Abstractions.CriticalSection.CriticalSection(UnlockFunc, true, opts);
        }
        catch (DbUpdateConcurrencyException entry) //handle db update concurrency exception that occured when locking
        {
            //invoke custom concurrency resolution
            var concurrencyResolutionTask = opts.OnConcurrencyResolutionHandle?.Invoke(entry);
            if (concurrencyResolutionTask is not null)
                await concurrencyResolutionTask;
            
            return new Abstractions.CriticalSection.CriticalSection(UnlockFunc, false, opts);
        }
        
        async Task UnlockFunc()
        {
            UnlockEntity(entity);
            await PersistChanges(cancellationToken);
        }
    }
    
    
    /// <summary>
    /// Gets current time
    /// </summary>
    /// <returns></returns>
    protected virtual DateTimeOffset GetCurrentTime() => timeProvider?.GetUtcNow() ?? DateTimeOffset.UtcNow;

    /// <summary>
    /// Persists changes using provided <typeparamref name="TDbContext"/>
    /// </summary>
    protected virtual async Task PersistChanges(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// Resets the lock on an <paramref name="entity"/>
    /// </summary>
    protected virtual void UnlockEntity<TTimeLock>(IHasTimeLock<TTimeLock> entity) where TTimeLock : struct, ITimeLock
    {
        entity.LockedUntil = null;
    }

    /// <summary>
    /// Checks if <paramref name="entity"/> is locked at <paramref name="currentTime"/>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="currentTime"></param>
    protected virtual bool IsUnlocked<TTimeLock>(IHasTimeLock<TTimeLock> entity, DateTimeOffset currentTime) where TTimeLock : struct, ITimeLock
    {
        return entity.LockedUntil?.IsNotLocked(currentTime) == false;
    }
}