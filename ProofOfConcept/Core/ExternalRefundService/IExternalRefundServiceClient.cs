using Core.Refund;

namespace Core.ExternalRefundService;

public interface IExternalRefundServiceClient
{
    Task<decimal> CreateRefundRequest(RefundEntity refundEntity, CancellationToken cancellationToken);
}