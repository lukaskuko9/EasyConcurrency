using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.HasTimeLock;
using EasyConcurrency.Abstractions.TimeLock;

namespace Stubs;

public class MyDbVersioning : IHasTimeLock
{
    public long Id { get; init; }
    public required Guid MyUniqueKey { get; init; }
    
    [ConcurrencyCheck]
    public TimeLock? LockedUntil { get; set; }

    [Timestamp] public byte[] Version { get; init; }
}