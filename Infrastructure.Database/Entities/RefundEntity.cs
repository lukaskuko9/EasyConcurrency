using System.Linq.Expressions;
using Core.PrimitiveTypeObsession;

namespace Infrastructure.Database.Entities;

public record RefundEntity(DatabaseId Id, string AccountKey, decimal Amount) : LockableEntity
{
    public bool IsProcessed { get; init; }
}