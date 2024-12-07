using EasyConcurrency.Abstractions;

namespace Stubs;

public class MyDbEntity : LockableEntity
{
    public long Id { get; init; }
    public required Guid MyUniqueKey { get; set; }
}