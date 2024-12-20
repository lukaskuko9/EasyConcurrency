using System.Linq.Expressions;

namespace EasyConcurrency.Abstractions.Entities.LockableEntity;

internal static class LockableEntityMethods
{
    internal static Expression<Func<T, bool>> IsNotLockedAsExpression<T>(DateTimeOffset now) where T : ILockableEntity
    {
        return lockableEntity=>lockableEntity.LockedUntil == null || lockableEntity.LockedUntil < now;
    }
}