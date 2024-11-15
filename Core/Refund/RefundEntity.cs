using Core.PrimitiveTypeObsession;

namespace Core.Refund;

public record RefundEntity(string AccountKey, decimal Amount)
{
   public DatabaseId Id { get; set; }
   public TimeLock? LockedUntil { get; set; }
}