using EasyConcurrency.Abstractions.Entities.LockableEntity;

namespace Stubs;

public class MyDbConcurrentEntity : LockableConcurrentEntity
{
    public long Id { get; set; }
    public required Guid MyUniqueKey { get; set; }
}