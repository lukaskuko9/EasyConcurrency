using EasyConcurrency.Abstractions.TimeLock;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework.CriticalSection;

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