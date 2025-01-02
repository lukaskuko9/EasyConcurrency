using EasyConcurrency.Abstractions.Entities.LockableEntity;

namespace Infrastructure.Database;

public class SampleEntity : LockableConcurrentEntity
{
    public long Id { get; set; }
    public Guid MyUuid { get; set; }
    public bool IsProcessed { get; set; }
}