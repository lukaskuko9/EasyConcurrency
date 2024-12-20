using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyConcurrency.Abstractions.Entities.LockableEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EasyConcurrency.EntityFramework.ConcurrentRepository;

/// <summary>
/// Provides an interface for concurrent repositories able to work with <see cref="ILockableEntity"/> types
/// </summary>
public interface IConcurrentRepository
{
    /// <summary>
    /// Locks the entities and calls <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> on database context to save the lock in database.
    /// </summary>
    /// <param name="entityToLock">Lockable entity to lock</param>
    /// <param name="lockTimeSpan">Duration of the lock</param>
    /// <param name="actionOnConcurrency">Action that will be called on concurrency</param>
    /// <param name="cancellationToken">CancellationToken to cancel the operation</param>
    /// <returns>Locked entity if lock was successful, otherwise null.</returns>
    Task<TLockableEntity?> LockAndSaveAsync<TLockableEntity>(TLockableEntity entityToLock,
        TimeSpan lockTimeSpan, Action<IReadOnlyList<EntityEntry>>? actionOnConcurrency = null, 
        CancellationToken cancellationToken = default) where TLockableEntity : class, ILockableEntity;

    /// <summary>
    /// Locks a collection of entities and calls <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> on database context to save the locks in database.
    /// </summary>
    /// <param name="entitiesToLock">Lockable entity to lock</param>
    /// <param name="lockTimeSpan">Duration of the lock</param>
    /// <param name="actionOnConcurrency">Action that will be called on concurrency</param>
    /// <param name="cancellationToken">CancellationToken to cancel the operation</param>
    /// <returns>A collection of locked entities, otherwise empty collection.</returns>
    Task<List<TLockableEntity>> LockAndSaveAsync<TLockableEntity>(
        IEnumerable<TLockableEntity> entitiesToLock, TimeSpan lockTimeSpan,
        Action<IReadOnlyList<EntityEntry>>? actionOnConcurrency = null,
        CancellationToken cancellationToken = default) where TLockableEntity : class, ILockableEntity;
    
    /// <summary>
    /// Inserts the entity and calls <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> on database context to save it in database.
    /// Handles exception that is thrown if the save violates unique index constraint
    /// </summary>
    /// <param name="entityToInsert">Entity to insert</param>
    /// <param name="token">CancellationToken to cancel the operation</param>
    /// <returns>If insert is successful returns the inserted entity, otherwise null.</returns>
    Task<bool> InsertAndSaveAsync<TLockableEntity>(TLockableEntity entityToInsert, CancellationToken token = default) where TLockableEntity : class, ILockableEntity;

    /// <summary>
    /// Inserts the entity and calls <see cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/> on database context to save it in database.
    /// Handles exception that is thrown if the save violates unique index constraint
    /// </summary>
    /// <param name="entitiesToInsert">Entity to insert</param>
    /// <param name="token">CancellationToken to cancel the operation</param>
    /// <returns>If insert is successful returns a collection of inserted entities, otherwise empty collection.</returns>
    Task<IReadOnlyCollection<TLockableEntity>> InsertAndSaveAsync<TLockableEntity>(IReadOnlyCollection<TLockableEntity> entitiesToInsert, CancellationToken token = default) where TLockableEntity : class, ILockableEntity;
    
}