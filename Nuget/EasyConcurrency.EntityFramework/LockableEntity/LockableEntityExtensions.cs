using System.Linq.Expressions;
using EasyConcurrency.Abstractions;

namespace EasyConcurrency.EntityFramework.LockableEntity;

public static class LockableEntityExtensions
{
    public static IQueryable<TLockableEntity> WhereIsNotLocked<TLockableEntity>(this IQueryable<TLockableEntity> queryable, DateTimeOffset now) where TLockableEntity: ILockableEntity
    {
        return queryable.Where(IsNotLocked<TLockableEntity>(now));
    }
    
    public static IQueryable<TLockableEntity> WhereIsNotLocked<TLockableEntity>(this IQueryable<TLockableEntity> queryable) where TLockableEntity: ILockableEntity
    {
        var now = DateTimeOffset.UtcNow;
        return queryable.Where(IsNotLocked<TLockableEntity>(now));
    }

    private static Expression<Func<TLockableEntity, bool>> IsNotLocked<TLockableEntity>(DateTimeOffset now) where TLockableEntity: ILockableEntity
    {
         return LockableEntityMethods.IsNotLockedAsExpression<TLockableEntity>(now);
    }
}