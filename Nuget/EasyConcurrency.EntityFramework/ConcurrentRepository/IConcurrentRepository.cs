using EasyConcurrency.Abstractions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EasyConcurrency.EntityFramework.ConcurrentRepository;

public interface IConcurrentRepository
{
    Task<TLockableEntity?> LockAndSaveAsync<TLockableEntity>(TLockableEntity? dbEntity,
        TimeSpan lockTimeSpan, Action<IReadOnlyList<EntityEntry>>? actionOnConcurrency = null, 
        CancellationToken cancellationToken = default) where TLockableEntity : class, ILockableEntity;

    Task<bool> InsertAndSaveAsync<TLockableEntity>(TLockableEntity entity, CancellationToken token = default) where TLockableEntity : class, ILockableEntity;
}