namespace Core.Refund;

public interface IRefundStorage
{
    Task InsertRefundIfNotExists(RefundEntity refundEntity, CancellationToken token = default);
    Task SetProcessedAsync(RefundEntity entityToUpdate, CancellationToken token = default);
    Task<RefundEntity?> GetAndLockSingleRefundToProcess(CancellationToken token);
}