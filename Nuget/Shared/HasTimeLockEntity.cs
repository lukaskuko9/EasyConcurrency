using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Tests.Shared;

public class HasTimeLockEntity : IHasTimeLock
{
    public long Id { get; init; }
    
    [ConcurrencyCheck]
    public TimeLock? LockedUntil { get; set; }
    
    [ConcurrencyCheck]
    public DateTimeOffset? Asd { get; set; }

    public string? TestParameterString { get; set; }
    
}