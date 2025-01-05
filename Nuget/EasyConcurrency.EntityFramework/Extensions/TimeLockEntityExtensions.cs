using System.Linq.Expressions;
using EasyConcurrency.Abstractions.Entities;

namespace EasyConcurrency.EntityFramework.Extensions;

/// <summary>
/// A set of extensions for <see cref="ITimeLockEntity"/> implementation types
/// </summary>
public static class TimeLockEntityExtensions
{
    /// <summary>
    /// Filters out the entities that are not locked.
    /// </summary>
    /// <param name="queryable">An <see cref="IQueryable"/> to filter</param>
    /// <param name="now">Date and time to use to determine if an entity is not locked</param>
    /// <typeparam name="TLockableEntity">Entity implementing <see cref="ITimeLockEntity"/></typeparam>
    /// <code>
    /// var dbEntityNotLocked = await databaseContext.MyLockableEntities
    /// .WhereIsNotLocked(DateTimeOffset.Now)
    /// .SingleAsync(myDbEntity => myDbEntity.Id == entityId);
    /// </code>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains elements from the input sequence that are not locked.
    /// </returns>
    public static IQueryable<TLockableEntity> WhereIsNotLocked<TLockableEntity>(this IQueryable<TLockableEntity> queryable, DateTimeOffset now) where TLockableEntity: ITimeLockEntity
    {
        return queryable.Where(IsNotLocked<TLockableEntity>(now));
    }
    
    /// <summary>
    /// Filters out the entities that are not locked using <see cref="DateTimeOffset.UtcNow"/> as current time.
    /// </summary>
    /// <param name="queryable">An <see cref="IQueryable{T}"/> to filter</param>
    /// <typeparam name="TLockableEntity">Entity implementing <see cref="ITimeLockEntity"/></typeparam>
    /// <code>
    /// var dbEntityNotLocked = await databaseContext.MyLockableEntities
    /// .WhereIsNotLocked()
    /// .SingleAsync(myDbEntity => myDbEntity.Id == entityId);
    /// </code>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains elements from the input sequence that are not locked.
    /// </returns>
    public static IQueryable<TLockableEntity> WhereIsNotLocked<TLockableEntity>(this IQueryable<TLockableEntity> queryable) where TLockableEntity: ITimeLockEntity
    {
        var now = DateTimeOffset.UtcNow;
        return queryable.Where(IsNotLocked<TLockableEntity>(now));
    }

    private static Expression<Func<TLockableEntity, bool>> IsNotLocked<TLockableEntity>(DateTimeOffset now) where TLockableEntity: ITimeLockEntity
    {
         return TimeLockEntityMethods.IsNotLockedAsExpression<TLockableEntity>(now);
    }
}