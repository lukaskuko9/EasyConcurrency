using Core.Refund;

namespace Job;

public class ConcurrentWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = serviceProvider.CreateScope();
        var refundService = scope.ServiceProvider.GetRequiredService<IRefundService>();

        await refundService.InsertRefundIfNotExists(new RefundEntity("key", 100), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await refundService.ProcessSingleRefund(stoppingToken);
        }
    }
}