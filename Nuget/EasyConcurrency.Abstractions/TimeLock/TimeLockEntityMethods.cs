using System.Linq.Expressions;
using EasyConcurrency.Abstractions.HasTimeLock;

namespace EasyConcurrency.Abstractions.TimeLock;

internal static class TimeLockEntityMethods
{
    internal static Expression<Func<T, bool>> IsNotLockedAsExpression<T>(DateTimeOffset now) where T : IHasTimeLock
    {
        return lockableEntity=>lockableEntity.LockedUntil == null || lockableEntity.LockedUntil < now;
    }
}