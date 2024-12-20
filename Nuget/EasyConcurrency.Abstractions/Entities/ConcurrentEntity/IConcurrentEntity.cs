using System.ComponentModel.DataAnnotations;

namespace EasyConcurrency.Abstractions.Entities.ConcurrentEntity;

/// <summary>
/// Entity base with ability to detect any concurrency in optimistic scenarios.
/// </summary>
public interface IConcurrentEntity
{
    /// <summary>
    /// Version stamp of current entity.
    /// </summary>
    [Timestamp] 
    public byte[] Version { get; init; }
}