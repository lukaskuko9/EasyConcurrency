using System.Linq.Expressions;
using EasyConcurrency.Abstractions.TimeLock;

namespace EasyConcurrency.Abstractions.Extensions;

/// <summary>
/// A set of extensions for <see cref="IHasTimeLock"/> implementation types
/// </summary>
public static class HasTimeLockExtensions
{
    /// <summary>
    /// Filters out the entities that are not locked.
    /// </summary>
    /// <param name="queryable">An <see cref="IQueryable"/> to filter</param>
    /// <param name="now">Date and time to use to determine if an entity is not locked</param>
    /// <typeparam name="TLockableEntity">Entity implementing <see cref="IHasTimeLock"/></typeparam>
    /// <code>
    /// var dbEntityNotLocked = await databaseContext.MyLockableEntities
    /// .WhereIsNotLocked(DateTimeOffset.Now)
    /// .SingleAsync(myDbEntity => myDbEntity.Id == entityId);
    /// </code>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains elements from the input sequence that are not locked.
    /// </returns>
    public static IQueryable<TLockableEntity> WhereIsNotLocked<TLockableEntity>(this IQueryable<TLockableEntity> queryable, DateTimeOffset now) where TLockableEntity: IHasTimeLock
    {
        return queryable.Where(IsNotLockedAsExpression<TLockableEntity>(now));
    } 

    /// <summary>
    /// Filters out the entities that are not locked using <see cref="DateTimeOffset.UtcNow"/> as current time.
    /// </summary>
    /// <param name="queryable">An <see cref="IQueryable{T}"/> to filter</param>
    /// <typeparam name="TLockableEntity">Entity implementing <see cref="IHasTimeLock"/></typeparam>
    /// <code>
    /// var dbEntityNotLocked = await databaseContext.MyLockableEntities
    /// .WhereIsNotLocked()
    /// .SingleAsync(myDbEntity => myDbEntity.Id == entityId);
    /// </code>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains elements from the input sequence that are not locked.
    /// </returns>
    public static IQueryable<TLockableEntity> WhereIsNotLocked<TLockableEntity>(this IQueryable<TLockableEntity> queryable) where TLockableEntity: IHasTimeLock
    {
        var now = DateTimeOffset.UtcNow;
        return queryable.Where(IsNotLockedAsExpression<TLockableEntity>(now));
    }

    private static Expression<Func<T, bool>> IsNotLockedAsExpression<T>(DateTimeOffset now) where T : IHasTimeLock
    {
        return lockableEntity => lockableEntity.LockedUntil == null || lockableEntity.LockedUntil < now;
    }
}