using EasyConcurrency.Abstractions.PessimisticLock;

namespace EasyConcurrency.Abstractions.TimeLock;

/// <summary>
/// Provides basic interface for determining, if an instance is locked or not.
/// </summary>
public interface ITimeLock : IPessimisticLock, IComparable<DateTimeOffset>, IComparable<DateTimeOffset?>, IEquatable<DateTimeOffset?>, IEquatable<DateTimeOffset>
{
    /// <summary>DatetimeOffset as a point in time, indicating when the lock expires. Null if lock is not set.</summary>
    public DateTimeOffset? Value { get; set; }
    
    /// <summary>
    /// Creates a <see cref="TimeLock"/> instance. 
    /// </summary>
    /// <param name="lockedUntil">The date and time expiration of the lock</param>
    /// <returns>New TimeLock instance specifying date and time until which the lock takes effect</returns>
    public static abstract ITimeLock Create(DateTimeOffset? lockedUntil);

    /// <summary>
    /// Checks whether this entity is not locked at specified time.
    /// </summary>
    /// <param name="now">Specifies the current time to be used when comparing if the entity is locked or not.</param>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    /// 
    public bool IsNotLocked(DateTimeOffset now);

    /// <summary>
    /// Checks whether this entity is locked at specified time provided by <paramref name="now"/> argument.
    /// </summary>
    /// <param name="now">Specifies the current time to be used when comparing if the entity is locked or not.</param>
    /// <returns>Returns true if entity is locked and therefore already claimed, otherwise false.</returns>
    public bool IsLocked(DateTimeOffset now);    
}