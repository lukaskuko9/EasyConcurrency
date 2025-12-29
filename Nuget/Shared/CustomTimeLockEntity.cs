using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Tests.Shared;

public class CustomTimeLockEntity : IHasTimeLock<CustomTimeLock>
{
    public long Id { get; init; }
    
    [ConcurrencyCheck]
    public CustomTimeLock? LockedUntil { get; set; }

    public string? TestParameterString { get; set; } = null;
    public Guid? TestParameterGuid { get; set; } = null;
}