using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public class RefundController(ILogger<RefundController> logger) : ControllerBase
{
    private static readonly Dictionary<string, decimal> Refunds = new();
    
    [HttpPost]
    public Task<IActionResult> Refund(RefundRequest request)
    {
        logger.LogInformation("Refund request {request} accepted", request);
        if(Refunds.ContainsKey(request.AccountKey) == false)
            Refunds.Add(request.AccountKey, request.Amount);
        else
            Refunds[request.AccountKey] += request.Amount;

        var totalRefunded = Refunds[request.AccountKey];
        logger.LogInformation("Total refunded for account {AccountKey} is {TotalRefunded}",
            request.AccountKey, totalRefunded
        );
        
        return Task.FromResult<IActionResult>(Ok(totalRefunded));
    }
}