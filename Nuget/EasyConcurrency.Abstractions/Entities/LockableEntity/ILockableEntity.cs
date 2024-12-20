using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.TimeLockNamespace;

namespace EasyConcurrency.Abstractions.Entities.LockableEntity;

/// <summary>
/// Provides interface for handling pessimistic concurrency scenarios of implementing class instance. 
/// </summary>
public interface ILockableEntity : IHasTimeLock
{
    /// <summary>
    /// Specifies the <see cref="TimeLock"/> until which the entity remains locked.
    /// If this property points to the future date and / or time,
    /// it is  locked, otherwise it is not locked.
    /// </summary>
    [ConcurrencyCheck]
    public TimeLock? LockedUntil { get; set; }
}