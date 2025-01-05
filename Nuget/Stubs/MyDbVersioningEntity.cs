using EasyConcurrency.Abstractions.Entities;

namespace Stubs;

public class MyDbVersioningEntity : TimeLockVersioningEntity
{
    public long Id { get; set; }
    public required Guid MyUniqueKey { get; set; }
}