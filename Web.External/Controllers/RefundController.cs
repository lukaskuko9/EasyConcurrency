using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public class RefundController(ILogger<RefundController> logger) : ControllerBase
{
    private record RefundLog(decimal Amount, DateTimeOffset RefundedOn);

    private static readonly Dictionary<string, List<RefundLog>> Refunds = new();
    [HttpPost]
    public Task<IActionResult> Refund(RefundRequest request)
    {
        logger.LogInformation("Refund request {request} accepted", request);
        
        if(Refunds.ContainsKey(request.AccountKey) == false)
            Refunds.Add(request.AccountKey, []);

        Refunds[request.AccountKey].Add(new RefundLog(request.Amount, DateTimeOffset.Now));

        var totalRefunded = Refunds[request.AccountKey].Select(refundLog => refundLog.Amount).Sum();
        logger.LogInformation("Total refunded for account {AccountKey} is {TotalRefunded}",
            request.AccountKey, totalRefunded
        );
        
        return Task.FromResult<IActionResult>(Ok(totalRefunded));
    }
    
    [HttpGet]
    public Task<IActionResult> GetRefunds()
    {
        if (Refunds.Count == 0)
            return Task.FromResult<IActionResult>(Ok("No refunds made"));

        var refundLogsBriefed = Refunds
            .Select(
                (kvp, _) =>
                    $"Account: {kvp.Key}; Refund logs: {Environment.NewLine}{GetBriefingOfLogs(kvp.Value)}{Environment.NewLine}"
            );
        
        var result = string.Join(Environment.NewLine, refundLogsBriefed);
        return Task.FromResult<IActionResult>(Ok(result));
    }

    [HttpPost]
    [Route("reset")]
    public Task<IActionResult> ResetRefunds()
    {
        Refunds.Clear();
        return Task.FromResult<IActionResult>(Ok());
    }

    private static string GetBriefingOfLogs(List<RefundLog> logs)
    {
        var logsStr = logs.Select(refundLog => $"Amount {refundLog.Amount} refunded on {refundLog.RefundedOn}");
        return string.Join(Environment.NewLine, logsStr);
    }
}