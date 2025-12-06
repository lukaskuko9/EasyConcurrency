using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.HasTimeLock;
using EasyConcurrency.Abstractions.TimeLock;

namespace Stubs;

public class MyHasTimeLock : IHasTimeLock
{
    public long Id { get; init; }
    
    [ConcurrencyCheck]
    public TimeLock? LockedUntil { get; set; }
}