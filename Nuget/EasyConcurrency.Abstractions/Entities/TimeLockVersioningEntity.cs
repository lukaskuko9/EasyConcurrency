using System.ComponentModel.DataAnnotations;

namespace EasyConcurrency.Abstractions.Entities;
    
/// Base class for pessimistic concurrency. Inherits from <see cref="TimeLockEntity"/> for locking functionality,
/// and implements <see cref="IVersioningEntity"/> interface for catching concurrency on any of the properties of inheriting class.
public abstract class TimeLockVersioningEntity : TimeLockEntity, IVersioningEntity
{
    /// <inheritdoc />
    [Timestamp] 
    public byte[] Version { get; init; } = [];
}