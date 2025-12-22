using EasyConcurrency.Abstractions.CriticalSection;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework.CriticalSection;

/// <summary>
/// Options to configure critical section behavior
/// </summary>
public record CriticalSectionEfOptions : CriticalSectionOptionsBase
{
    /// <summary>
    /// Specifies custom asynchronous action to be executed when a concurrency happens when acquiring a lock.
    /// This can be used to resolve the concurrency conflict  
    /// </summary>
    public Func<DbUpdateConcurrencyException, Task>? OnConcurrencyResolutionHandle { get; set; }
}