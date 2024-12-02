using Public;

namespace UnitTests.Database;

public class MyDbEntity : LockableEntity
{
    public long Id { get; init; }
    public required Guid MyUniqueKey { get; set; }
}