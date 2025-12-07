using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Abstractions.HasTimeLock;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework.Entities;

/// <summary>
/// Represents a critical section in code. When disposed will unlock <paramref name="hasTimeLock"></paramref>
/// </summary>
/// <param name="hasTimeLock">Entity with a <see cref="TimeLock"/> to enter the critical section</param>
/// <param name="isLockAcquired">True if lock was successfully acquired for this entity, otherwise false.</param>
/// <param name="autoReleaseLockOnSectionExit">Automatically releases lock and commits to database when leaving critical section</param>
/// <param name="dbContext">Database context</param>
/// <param name="cancellationToken">Cancellation token to cancel the operation</param>
/// <typeparam name="TDbContext">Database context</typeparam>
public class CriticalSection<TDbContext>(
    IHasTimeLock hasTimeLock,
    TDbContext dbContext,
    bool isLockAcquired,
    bool autoReleaseLockOnSectionExit,
    CancellationToken cancellationToken) : IDisposable, IAsyncDisposable
    where TDbContext : DbContext
{
    /// <summary>
    /// If true, lock was successfully acquired, and it is safe to continue critical section code.
    /// If false, lock was not acquired successfully and critical section code should not usually continue.
    /// </summary>
    public bool IsLockAcquired { get; init; } = isLockAcquired;
    
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (autoReleaseLockOnSectionExit == false) 
            return;
        
        hasTimeLock.Unlock();
        dbContext.SaveChanges();
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync()
    {
        if (autoReleaseLockOnSectionExit == false) 
            return;
        
        hasTimeLock.Unlock();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

// ReSharper disable once UnusedTypeParameter
/// <summary>
/// Service for safely handling critical sections with pessimistic concurrency.
/// </summary>
/// <typeparam name="TDbContext">Database context</typeparam>
public interface ICriticalSectionService<TDbContext> where TDbContext : DbContext
{
    /// <summary>
    /// Enters the critical section. Before entering, a lock will try to be acquired for <paramref name="entityWithLock"/>.
    /// Handles <see cref="DbUpdateConcurrencyException"/> without crashing.
    /// When exiting critical section, the lock will be released automatically - unless explicitly configured otherwise in <see cref="CriticalSectionOptions"/>.
    /// </summary>
    /// <param name="entityWithLock">Entity with a time lock approaching critical section</param>
    /// <param name="lockForTime">How long will lock be acquired for</param>
    /// <param name="criticalSectionOptions">Options to configure behavior</param>
    /// <param name="token">Cancellation token to cancel the operation</param>
    /// <typeparam name="TTimeLockEntity">Entity with a time lock</typeparam>
    /// <remarks>You should always check if the lock was acquired. See <see cref="CriticalSection{TDbContext}.IsLockAcquired"/></remarks>
    /// <code>
    /// await using (var criticalSection = await criticalSectionService.BeginCriticalSectionAndCommitAsync(entityToLock, TimeSpan.FromMinutes(1), opts))
    /// {
    ///     if (criticalSection.IsLockAcquired == false)
    ///     {
    ///         return false;
    ///     }
    ///
    ///     //critical section code
    ///     await db.SaveChangesAsync(); //optionally persist database changes before exiting critical section
    /// }
    /// </code>
    /// <returns><see cref="CriticalSection{TDbContext}"/> instance</returns>
    Task<CriticalSection<TDbContext>> BeginCriticalSectionAndCommitAsync<TTimeLockEntity>(TTimeLockEntity entityWithLock,
        TimeSpan lockForTime, Action<CriticalSectionOptions>? criticalSectionOptions = null, CancellationToken token = default)
        where TTimeLockEntity : IHasTimeLock;
}

/// <inheritdoc/>
public class CriticalSectionService<TDbContext>(TDbContext dbContext) : ICriticalSectionService<TDbContext> where TDbContext : DbContext
{
    /// <inheritdoc/>
    public async Task<CriticalSection<TDbContext>> BeginCriticalSectionAndCommitAsync<TTimeLockEntity>(TTimeLockEntity entityWithLock, TimeSpan lockForTime, Action<CriticalSectionOptions>? criticalSectionOptions = null, CancellationToken token = default)
        where TTimeLockEntity : IHasTimeLock
    {
        var opts = new CriticalSectionOptions();
        //lock the entity
        try
        {
            if (entityWithLock.LockedUntil?.Value >= DateTimeOffset.UtcNow)
                return new CriticalSection<TDbContext>(entityWithLock, dbContext, false, false, token);
            
            criticalSectionOptions?.Invoke(opts);
            
            entityWithLock.LockedUntil = DateTimeOffset.UtcNow.Add(lockForTime);
            await dbContext.SaveChangesAsync(token);
            
            return new CriticalSection<TDbContext>(entityWithLock, dbContext, true, opts.AutoUnlockOnCriticalSectionExit, token);
        }
        catch (DbUpdateConcurrencyException entry)
        {
            opts.OnConcurrencyResolutionHandle?.Invoke(entry);
            return new CriticalSection<TDbContext>(entityWithLock, dbContext, false, false, token);
        }
    }
}

/// <summary>
/// Options to configure critical section behavior
/// </summary>
public record CriticalSectionOptions
{
    /// <summary>
    /// If true, releases a lock when exiting / disposing critical section.
    /// </summary>
    public bool AutoUnlockOnCriticalSectionExit { get; set; } = true;
    
    /// <summary>
    /// Specifies custom action to be executed when a concurrency happens when acquiring a lock.
    /// This can be used to resolve the concurrency conflict  
    /// </summary>
    public Action<DbUpdateConcurrencyException>? OnConcurrencyResolutionHandle { get; set; }
}