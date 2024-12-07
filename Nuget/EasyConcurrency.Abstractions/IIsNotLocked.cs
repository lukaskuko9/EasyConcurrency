namespace EasyConcurrency.Abstractions;

/// <summary>
/// Provides basic interface for determining, if an instance is locked or not.
/// </summary>
public interface IIsNotLocked
{
    /// <summary>
    /// Checks whether this entity is not locked using <see cref="DateTimeOffset.UtcNow"/> as current time.
    /// </summary>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public bool IsNotLocked();
    
    /// <summary>
    /// Checks whether this entity is not locked at specified time.
    /// </summary>
    /// <param name="now">Specifies the current time to be used when comparing if the entity is locked or not.</param>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed,
    /// otherwise false.
    /// </returns>
    public bool IsNotLocked(DateTimeOffset now);

}