using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Abstractions.HasTimeLock;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework.Entities;

/// <summary>
/// Represents a critical section in code. When disposed will unlock <param name="hasTimeLock"></param>
/// </summary>
/// <param name="hasTimeLock"></param>
/// <param name="dbContext"></param>
/// <param name="cancellationToken"></param>
/// <typeparam name="TDbContext"></typeparam>
public class CriticalSection<TDbContext>(
    IHasTimeLock hasTimeLock,
    TDbContext dbContext,
    bool lockAcquired,
    bool autoUnlockOnSectionExit,
    CancellationToken cancellationToken) : IDisposable, IAsyncDisposable
    where TDbContext : DbContext
{
    public bool LockAcquired { get; init; } = lockAcquired;
    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (autoUnlockOnSectionExit == false) 
            return;
        hasTimeLock.Unlock();
        dbContext.SaveChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (autoUnlockOnSectionExit == false) 
            return;
        
        hasTimeLock.Unlock();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

// ReSharper disable once UnusedTypeParameter
public interface ICriticalSectionService<TDbContext> where TDbContext : DbContext
{
    Task<CriticalSection<TDbContext>> BeginCriticalSectionAndCommitAsync<TTimeLockEntity>(TTimeLockEntity hasTimeLock,
        TimeSpan lockForTime, Action<CriticalSectionOptions>? criticalSectionOptions = null, CancellationToken token = default)
        where TTimeLockEntity : IHasTimeLock;
}

public class CriticalSectionService<TDbContext>(TDbContext dbContext) : ICriticalSectionService<TDbContext> where TDbContext : DbContext
{
    public async Task<CriticalSection<TDbContext>> BeginCriticalSectionAndCommitAsync<TTimeLockEntity>(TTimeLockEntity hasTimeLock, TimeSpan lockForTime, Action<CriticalSectionOptions>? criticalSectionOptions = null, CancellationToken token = default)
        where TTimeLockEntity : IHasTimeLock
    {
        var opts = new CriticalSectionOptions();
        //lock the entity
        try
        {
            if (hasTimeLock.LockedUntil?.Value >= DateTimeOffset.UtcNow)
                return new CriticalSection<TDbContext>(hasTimeLock, dbContext, false, false, token);
            
            criticalSectionOptions?.Invoke(opts);
            
            hasTimeLock.LockedUntil = DateTimeOffset.UtcNow.Add(lockForTime);
            await dbContext.SaveChangesAsync(token);
            
            return new CriticalSection<TDbContext>(hasTimeLock, dbContext, true, opts.AutoUnlockOnCriticalSectionExit, token);
        }
        catch (DbUpdateConcurrencyException entry)
        {
            opts.OnConcurrencyResolutionHandle?.Invoke(entry);
            return new CriticalSection<TDbContext>(hasTimeLock, dbContext, false, false, token);
        }
    }
}

public record CriticalSectionOptions
{
    public bool AutoUnlockOnCriticalSectionExit { get; set; } = true;
    public Action<DbUpdateConcurrencyException>? OnConcurrencyResolutionHandle { get; set; }
}