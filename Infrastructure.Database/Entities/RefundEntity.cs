using Core.PrimitiveTypeObsession;

namespace Infrastructure.Database.Entities;

public record RefundEntity(DatabaseId Id, string AccountKey, decimal Amount) : ConcurrentEntity
{
    public bool IsProcessed { get; init; }
}