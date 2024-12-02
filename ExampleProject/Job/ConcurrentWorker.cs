using Core.Refund;

namespace Job;

public class ConcurrentWorker(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {       
            
            var scope = serviceProvider.CreateScope();
            var refundService = scope.ServiceProvider.GetRequiredService<IRefundProcessingService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ConcurrentWorker>>();

            /*  var inserted = await refundService.InsertRefundIfNotExists(new RefundEntity("key", 100), stoppingToken);
              if(inserted == false)
                  logger.LogInformation("Failed to insert refund entity!");
              */
            await refundService.ProcessSingleRefund(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }
}