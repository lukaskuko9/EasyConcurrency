using System.Linq.Expressions;

namespace Public;

public static class TimeLockMethods
{
    public static Expression<Func<T, bool>> IsNotLocked<T>() where T : LockableEntity
    {
        var now = DateTimeOffset.Now;
        var parameter = Expression.Parameter(typeof(T), "p");
        var lambda = Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                Expression.Equal(
                    Expression.Convert(Expression.Property(parameter, nameof(LockableEntity.LockedUntil)),
                        typeof(DateTimeOffset?)),
                    Expression.Constant(null, typeof(DateTimeOffset?))
                ),
                Expression.LessThan(
                    Expression.Convert(Expression.Property(parameter, nameof(LockableEntity.LockedUntil)),
                        typeof(DateTimeOffset)),
                    Expression.Constant(now, typeof(DateTimeOffset))
                )
            )
            , parameter
        );
        return lambda;
    }
}