namespace Core.Refund;

public interface IRefundProcessingRepository
{
    Task<bool> InsertRefundIfNotExists(RefundEntity refundEntity, CancellationToken token = default);
    Task DeleteAsync(RefundEntity entityToUpdate, CancellationToken token = default);
    Task<RefundEntity?> GetAndLockFirstOrDefaultAsync(CancellationToken token);
}