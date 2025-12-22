using EasyConcurrency.Abstractions.CriticalSection;
using EasyConcurrency.Abstractions.TimeLock;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework.CriticalSection;

/// <inheritdoc/>
public class CriticalSectionService<TDbContext>(TDbContext dbContext, TimeProvider? timeProvider = null) : ICriticalSectionService<CriticalSectionOptions> where TDbContext : DbContext
{
    private DateTimeOffset Now => timeProvider?.GetUtcNow() ?? DateTimeOffset.UtcNow;
    
    /// <inheritdoc/>
    public async Task<Abstractions.CriticalSection.CriticalSection> BeginCriticalSectionAndCommitAsync<TTimeLock>(IHasTimeLock<TTimeLock> entity, TimeSpan lockForTime,
        Action<CriticalSectionOptions>? criticalSectionOptions = null, CancellationToken token = default) where TTimeLock : struct, ITimeLock
    {
        var opts = new CriticalSectionOptions
        {
            AutoUnlockOnCriticalSectionExit = true
        };
        
        try
        {
            //if entity is already locked we cannot proceed
            if (entity.LockedUntil?.IsNotLocked(Now) == false)
                return new Abstractions.CriticalSection.CriticalSection(UnlockFunc, false, opts);
            
            criticalSectionOptions?.Invoke(opts);

            //try lock the entity
            entity.LockedUntil = (TTimeLock)TTimeLock.Create(Now.Add(lockForTime));
            await dbContext.SaveChangesAsync(token);
            
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
            entity.LockedUntil = null;
            await dbContext.SaveChangesAsync(token);
        }
    }
}