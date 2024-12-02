using System.ComponentModel.DataAnnotations;

namespace Public;

public abstract class ConcurrentEntity
{
    [Timestamp] 
    public byte[] Version { get; init; } = [];
}