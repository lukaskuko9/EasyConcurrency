using EasyConcurrency.Abstractions.Entities.LockableEntity;

namespace Stubs;

public class MyLockableEntity : LockableEntity
{
    public long Id { get; init; }
}