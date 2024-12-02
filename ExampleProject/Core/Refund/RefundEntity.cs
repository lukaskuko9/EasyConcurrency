using Core.PrimitiveTypeObsession;

namespace Core.Refund;

public record RefundEntity(string AccountKey, decimal Amount)
{
   public DatabaseId Id { get; init; }
   public required bool IsProcessed { get; set; }
}