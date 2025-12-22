using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.TimeLock;

namespace Stubs;

public class MyLockableEntity : IHasTimeLock
{
    public long Id { get; init; }
    
    [ConcurrencyCheck]
    public TimeLock? LockedUntil { get; set; }
    
    [ConcurrencyCheck]
    public DateTimeOffset? Asd { get; set; }

    public string? TestParameterString { get; set; } = null;
    public Guid? TestParameterGuid { get; set; } = null;
}