using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.HasTimeLock;
using EasyConcurrency.Abstractions.TimeLock;

namespace Stubs;

public class MyDbVersioning : IHasTimeLock
{
    public long Id { get; set; }
    public required Guid MyUniqueKey { get; set; }
    
    [ConcurrencyCheck]
    public TimeLock? LockedUntil { get; set; }
    
    [Timestamp]
    public byte[] Version { get; init; }
}