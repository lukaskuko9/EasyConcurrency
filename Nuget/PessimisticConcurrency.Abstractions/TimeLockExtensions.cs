namespace EntityFrameworkCore.PessimisticConcurrency.Abstractions;

public static class TimeLockExtensions
{
    public static IQueryable<TLockableEntity> WhereIsNotLocked<TLockableEntity>(this IQueryable<TLockableEntity> queryable)
        where TLockableEntity : LockableEntity
    {
        var now = DateTimeOffset.Now;
        return queryable.Where(entity => entity.IsNotLocked(now));
    }
}