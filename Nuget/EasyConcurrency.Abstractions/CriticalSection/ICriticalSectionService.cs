using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.CriticalSection;

/// <summary>
/// Service for safely handling critical sections with pessimistic concurrency.
/// </summary>
public interface ICriticalSectionService<out TOptions> where TOptions: CriticalSectionOptionsBase
{
    /// <summary>
    /// Enters the critical section. Before entering, a lock will try to be acquired for <paramref name="entityWithLock"/>.
    /// When exiting critical section, the lock will be released automatically - unless explicitly configured otherwise.
    /// </summary>
    /// <param name="entityWithLock">Entity with a time lock approaching critical section</param>
    /// <param name="lockForTime">How long will lock be acquired for</param>
    /// <param name="criticalSectionOptions">Options to configure behavior</param>
    /// <param name="token">Cancellation token to cancel the operation</param>
    /// <remarks>You should always check if the lock was acquired. See <see cref="CriticalSection.IsLockAcquired"/></remarks>
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
    /// <returns><see cref="CriticalSection"/> instance</returns>
    Task<CriticalSection> BeginCriticalSectionAndCommitAsync<TTimeLock>(IHasTimeLock<TTimeLock> entityWithLock,
        TimeSpan lockForTime, Action<TOptions>? criticalSectionOptions = null,
        CancellationToken token = default) where TTimeLock : struct, ITimeLock;
}