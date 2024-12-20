using System.ComponentModel.DataAnnotations;

namespace EasyConcurrency.Abstractions.Entities.LockableEntity;

public abstract class LockableConcurrentEntity : LockableEntity, ConcurrentEntity.IConcurrentEntity
{
    [Timestamp] 
    public byte[] Version { get; init; } = [];
}