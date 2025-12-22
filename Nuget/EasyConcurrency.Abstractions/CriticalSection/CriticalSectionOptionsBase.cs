namespace EasyConcurrency.Abstractions.CriticalSection;

/// <summary>
/// Options to configure critical section behavior
/// </summary>
public abstract record CriticalSectionOptionsBase 
{
    /// <summary>
    /// If true, releases a lock when exiting / disposing critical section.
    /// </summary>
    public bool AutoUnlockOnCriticalSectionExit { get; set; }
}