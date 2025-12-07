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
    Task<CriticalSection<TDbContext>> BeginCriticalSectionAndCommitAsync(IHasTimeLock hasTimeLock, TimeSpan lockForTime, bool autoUnlockOnSectionExit, CancellationToken token = default);
}

public class CriticalSectionService<TDbContext>(TDbContext dbContext) : ICriticalSectionService<TDbContext> where TDbContext : DbContext
{
    public async Task<CriticalSection<TDbContext>> BeginCriticalSectionAndCommitAsync(IHasTimeLock hasTimeLock, TimeSpan lockForTime, bool autoUnlockOnSectionExit = true, CancellationToken token = default)
    {
        //lock the entity
        try
        {
            if (hasTimeLock.LockedUntil?.Value >= DateTimeOffset.UtcNow)
                return new CriticalSection<TDbContext>(hasTimeLock, dbContext, false, autoUnlockOnSectionExit, token);
            
            hasTimeLock.SetLock(lockForTime);
            await dbContext.SaveChangesAsync(token);
            return new CriticalSection<TDbContext>(hasTimeLock, dbContext, true, autoUnlockOnSectionExit, token);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new CriticalSection<TDbContext>(hasTimeLock, dbContext, false, autoUnlockOnSectionExit, token);
        }
    }
}