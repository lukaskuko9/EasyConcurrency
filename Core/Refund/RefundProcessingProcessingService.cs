using Core.ExternalRefundService;

namespace Core.Refund;

public class RefundProcessingProcessingService(IRefundProcessingRepository refundProcessingRepository, IExternalRefundServiceClient externalRefundServiceClient) : IRefundProcessingService
{
    public async Task ProcessSingleRefund(CancellationToken token)
    {
        var refundToProcess = await refundProcessingRepository.GetAndLockFirstOrDefaultAsync(token);
        if (refundToProcess is null)
            return; 
        
        var totalAmountRefunded = await externalRefundServiceClient.CreateRefundRequest(refundToProcess, token);
        await refundProcessingRepository.DeleteAsync(refundToProcess, token);
    }

    public async Task<bool> InsertRefundIfNotExists(RefundEntity refundEntity, CancellationToken token)
    {
        return await refundProcessingRepository.InsertRefundIfNotExists(refundEntity, token);
    }
}