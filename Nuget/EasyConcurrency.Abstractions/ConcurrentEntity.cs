using System.ComponentModel.DataAnnotations;

namespace EasyConcurrency.Abstractions;

public abstract class ConcurrentEntity
{
    [Timestamp] 
    public byte[] Version { get; } = [];
}