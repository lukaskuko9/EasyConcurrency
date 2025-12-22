using System.ComponentModel.DataAnnotations;

namespace EasyConcurrency.Abstractions.TimeLock;

/// <inheritdoc/>
public interface IHasTimeLock : IHasTimeLock<TimeLock>;

/// <summary>
/// Provides interface for handling pessimistic concurrency scenarios of implementing class instance. 
/// </summary>
/// <typeparam name="TTimeLock"><see cref="ITimeLock"/> type to use</typeparam>
public interface IHasTimeLock<TTimeLock> where TTimeLock : struct, ITimeLock
{
    /// <summary>
    /// Specifies the <typeparamref name="TTimeLock"/> until which the entity remains locked.
    /// If this property points to the future date and / or time,
    /// it is  locked, otherwise it is not locked.
    /// </summary>
    [ConcurrencyCheck]
    public TTimeLock? LockedUntil { get; set; }
}