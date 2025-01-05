using System.Linq.Expressions;

namespace EasyConcurrency.Abstractions.Entities;

internal static class TimeLockEntityMethods
{
    internal static Expression<Func<T, bool>> IsNotLockedAsExpression<T>(DateTimeOffset now) where T : ITimeLockEntity
    {
        return lockableEntity=>lockableEntity.LockedUntil == null || lockableEntity.LockedUntil < now;
    }
}