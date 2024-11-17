namespace Core.Refund;

public interface IRefundProcessingService
{
    Task ProcessSingleRefund(CancellationToken token);
    Task<bool> InsertRefundIfNotExists(RefundEntity refundEntity, CancellationToken token);
}