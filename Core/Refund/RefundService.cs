using Core.ExternalRefundService;

namespace Core.Refund;

public class RefundService(IRefundStorage refundStorage, IExternalRefundServiceClient externalRefundServiceClient) : IRefundService
{
    public async Task ProcessSingleRefund(CancellationToken token)
    {
        var refundToProcess = await refundStorage.GetAndLockSingleRefundToProcess(token);
        if (refundToProcess is null)
            return; 
        
        //_ = await externalRefundServiceClient.CreateRefundRequest(refundToProcess, token);
        await refundStorage.SetProcessedAsync(refundToProcess, token);
    }

    public async Task InsertRefundIfNotExists(RefundEntity refundEntity, CancellationToken token)
    {
        await refundStorage.InsertRefundIfNotExists(refundEntity, token);
    }
}