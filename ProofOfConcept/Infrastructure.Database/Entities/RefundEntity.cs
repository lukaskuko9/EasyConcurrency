using Core.PrimitiveTypeObsession;
using EasyConcurrency.Abstractions;

namespace Infrastructure.Database.Entities;

public class RefundEntity : LockableEntity
{
    public DatabaseId Id { get; init; }
    public required string AccountKey { get; init; }
    public required decimal Amount { get; init; }
    public required bool IsProcessed { get; set; }
}