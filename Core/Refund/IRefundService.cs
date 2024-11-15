namespace Core.Refund;

public interface IRefundService
{
    Task ProcessSingleRefund(CancellationToken token);
    Task InsertRefundIfNotExists(RefundEntity refundEntity, CancellationToken token);
}