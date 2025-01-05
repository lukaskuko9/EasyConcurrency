using System.ComponentModel.DataAnnotations;

namespace EasyConcurrency.Abstractions.Entities;

/// <summary>
/// Entity base with ability to detect any concurrency in optimistic scenarios.
/// </summary>
public interface IVersioningEntity
{
    /// <summary>
    /// Version stamp of current entity.
    /// 
    /// When an entity is modified and changes are persisted to target data source,
    /// the value in this property will be changed.
    /// 
    /// When this value is fetched from data source and later saved while
    /// this value does not match with the value in data source, concurrency exception will be raised.
    /// </summary>
    [Timestamp] 
    public byte[] Version { get; init; }
}