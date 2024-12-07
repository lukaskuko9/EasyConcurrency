using EasyConcurrency.Abstractions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EasyConcurrency.EntityFramework.ConcurrentRepository;

public interface IConcurrentRepository
{
    Task<TConcurrentEntity?> LockAndSaveAsync<TConcurrentEntity>(TConcurrentEntity? dbEntity,
        TimeSpan lockTimeSpan, Action<IReadOnlyList<EntityEntry>>? actionOnConcurrency = null, 
        CancellationToken cancellationToken = default) where TConcurrentEntity : class, ILockableEntity;

    Task<bool> InsertAndSaveAsync<TConcurrentEntity>(TConcurrentEntity entity, CancellationToken token = default) where TConcurrentEntity : class, ILockableEntity;
}