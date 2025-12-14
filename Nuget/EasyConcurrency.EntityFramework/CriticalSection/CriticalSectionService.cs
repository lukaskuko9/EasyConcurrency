using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Abstractions.HasTimeLock;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework.CriticalSection;

/// <inheritdoc/>
public sealed class CriticalSectionService<TDbContext>(TDbContext dbContext, TimeProvider? timeProvider = null) : ICriticalSectionService<TDbContext> where TDbContext : DbContext
{
    
    private DateTimeOffset Now => timeProvider?.GetUtcNow() ?? DateTimeOffset.UtcNow;
    /// <inheritdoc/>
    public async Task<CriticalSection<TDbContext>> BeginCriticalSectionAndCommitAsync<TTimeLockEntity>(TTimeLockEntity entityWithLock, TimeSpan lockForTime, Action<CriticalSectionOptions>? criticalSectionOptions = null, CancellationToken token = default)
        where TTimeLockEntity : IHasTimeLock
    {
        var opts = new CriticalSectionOptions();
        //try lock the entity
        try
        {
            if (entityWithLock.IsNotLocked(Now) == false)
                return new CriticalSection<TDbContext>(entityWithLock, dbContext, false, false, token);
            
            criticalSectionOptions?.Invoke(opts);
            
            entityWithLock.LockedUntil = Now.Add(lockForTime);
            await dbContext.SaveChangesAsync(token);
            
            return new CriticalSection<TDbContext>(entityWithLock, dbContext, true, opts.AutoUnlockOnCriticalSectionExit, token);
        }
        catch (DbUpdateConcurrencyException entry)
        {
            //handle db update concurrency exception that occured when locking
            opts.OnConcurrencyResolutionHandle?.Invoke(entry);
            return new CriticalSection<TDbContext>(entityWithLock, dbContext, false, false, token);
        }
    }
}