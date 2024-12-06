using System.Linq.Expressions;
using EasyConcurrency.Abstractions;

namespace EasyConcurrency.EntityFramework;

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
    
    public static Expression<Func<TLockableEntity, bool>> IsNotLocked<TLockableEntity>() where TLockableEntity: ILockableEntity
    {
        var now = DateTimeOffset.UtcNow;
        return IsNotLocked<TLockableEntity>(now);
    }
    
    public static Expression<Func<TLockableEntity, bool>> IsNotLocked<TLockableEntity>(DateTimeOffset now) where TLockableEntity: ILockableEntity
    {
         return TimeLockMethods.IsNotLockedAsExpression<TLockableEntity>(now);
    }
}