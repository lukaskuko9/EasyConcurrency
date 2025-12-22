namespace EasyConcurrency.Abstractions.PessimisticLock;

/// <summary>
/// Base interface for basic pessimistic scenarios.
/// </summary>
public interface IPessimisticLock
{
    /// <summary>
    /// Checks whether this entity is not locked.
    /// </summary>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed, otherwise false.</returns>
    public bool IsNotLocked();
    
    /// <summary>
    /// Checks whether this entity is locked.
    /// </summary>
    /// <returns>Returns true if entity is not locked and therefore free to be claimed, otherwise false.</returns>
    public bool IsLocked();
}