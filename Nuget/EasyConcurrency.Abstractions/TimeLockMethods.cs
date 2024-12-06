using System.Linq.Expressions;

namespace EasyConcurrency.Abstractions;

public static class TimeLockMethods
{
    public static bool IsNotLocked<T>(this T entity, DateTimeOffset now) where T : ILockableEntity
    {
        return IsNotLockedBase<T>(now)(entity);
    }
    public static bool IsNotLocked<T>(this T entity) where T : ILockableEntity
    {
        var now = DateTimeOffset.UtcNow;
        return entity.IsNotLocked<T>(now);
    }

    internal static Func<T, bool> IsNotLockedBase<T>(DateTimeOffset now) where T : ILockableEntity
    {
        return lockableEntity=>lockableEntity.LockedUntil == null || lockableEntity.LockedUntil < now;
    }
    
    internal static Expression<Func<T, bool>> IsNotLockedAsExpression<T>(DateTimeOffset now) where T : ILockableEntity
    {
        return lockableEntity=>lockableEntity.LockedUntil == null || lockableEntity.LockedUntil < now;
    }
    
    public static Expression<Func<T, bool>> IsNotLocked<T>() where T : ILockableEntity
    {
        var now = DateTimeOffset.UtcNow;
        var parameter = Expression.Parameter(typeof(T), "p");
        var lambda = Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                Expression.Equal(
                    Expression.Convert(Expression.Property(parameter, nameof(ILockableEntity.LockedUntil)),
                        typeof(DateTimeOffset?)),
                    Expression.Constant(null, typeof(DateTimeOffset?))
                ),
                Expression.LessThan(
                    Expression.Convert(Expression.Property(parameter, nameof(ILockableEntity.LockedUntil)),
                        typeof(DateTimeOffset)),
                    Expression.Constant(now, typeof(DateTimeOffset))
                )
            )
            , parameter
        );
        return lambda;
    }
}