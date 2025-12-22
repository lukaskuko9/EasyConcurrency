namespace EasyConcurrency.Abstractions.CriticalSection;

/// <summary>
/// Represents a critical section in code. When disposed will unlock
/// </summary>
/// <param name="releaseMethod">Method defining the release of lock. Will be invoked when disposing this object instance.
/// Can be configured not to autorelease with <see cref="CriticalSectionOptionsBase.AutoUnlockOnCriticalSectionExit"/>
/// </param>
/// <param name="isLockAcquired">True if lock was successfully acquired for this entity, otherwise false.</param>
/// <param name="opts">Options to set behavior on this critical section.</param>
public class CriticalSection(
    Func<Task> releaseMethod,
    bool isLockAcquired,
    CriticalSectionOptionsBase opts) : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// If true, lock was successfully acquired, and it is safe to continue critical section code.
    /// If false, lock was not acquired successfully and critical section code should not usually continue.
    /// </summary>
    public bool IsLockAcquired { get; } = isLockAcquired;
    
    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (IsLockAcquired == false || opts.AutoUnlockOnCriticalSectionExit == false) 
            return;

        releaseMethod().RunSynchronously();
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync()
    {
        if (IsLockAcquired == false || opts.AutoUnlockOnCriticalSectionExit == false) 
            return;

        await releaseMethod();
    }
}