using System.ComponentModel.DataAnnotations;

namespace EasyConcurrency.Abstractions;

/// <summary>
/// Entity base with ability to detect concurrency in optimistic scenarios.
/// </summary>
public abstract class ConcurrentEntity
{
    /// <summary>
    /// Version stamp of current entity.
    /// </summary>
    [Timestamp] 
    public byte[] Version { get; } = [];
}