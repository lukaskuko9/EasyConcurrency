using System.ComponentModel.DataAnnotations;

namespace EasyConcurrency.Abstractions.Entities.LockableEntity;
    
/// Base class for pessimistic concurrency. Inherits from <see cref="LockableEntity"/> for locking functionality,
/// and implements <see cref="ConcurrentEntity.IConcurrentEntity"/> interface for catching concurrency when locking,
/// to make sure only one thread can lock it.
public abstract class LockableConcurrentEntity : LockableEntity, ConcurrentEntity.IConcurrentEntity
{
    /// <inheritdoc />
    [Timestamp] 
    public byte[] Version { get; } = [];
}