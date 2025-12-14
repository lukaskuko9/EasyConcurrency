using EasyConcurrency.Abstractions.Extensions;
using EasyConcurrency.Abstractions.HasTimeLock;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework.CriticalSection;

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
    public bool IsLockAcquired { get; } = isLockAcquired;
    
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