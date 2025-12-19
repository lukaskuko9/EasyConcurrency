using System.ComponentModel.DataAnnotations;
using EasyConcurrency.Abstractions.IsTimeLock;
using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.HasTimeLock;

/// <inheritdoc/>
public interface IHasTimeLock : IHasTimeLock<TimeLock.TimeLock>;

/// <summary>
/// Provides interface for handling pessimistic concurrency scenarios of implementing class instance. 
/// </summary>
/// <typeparam name="TTimeLock"><see cref="IIsTimeLock"/> type to use</typeparam>
public interface IHasTimeLock<TTimeLock> where TTimeLock : struct, IIsTimeLock
{
    /// <summary>
    /// Specifies the <see cref="TimeLock"/> until which the entity remains locked.
    /// If this property points to the future date and / or time,
    /// it is  locked, otherwise it is not locked.
    /// </summary>
    [ConcurrencyCheck]
    public TTimeLock? LockedUntil { get; set; }
}