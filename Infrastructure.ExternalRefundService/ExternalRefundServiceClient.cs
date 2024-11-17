using System.Globalization;
using System.Text;
using System.Text.Json;
using Core.ExternalRefundService;
using Core.Refund;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalRefundService;



public class ExternalRefundServiceClient(IOptions<ExternalRefundServiceOptions> options) : IExternalRefundServiceClient
{
    public async Task<decimal> CreateRefundRequest(RefundEntity refundEntity, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(refundEntity);
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(options.Value.Url);
        var result = await httpClient.PostAsync("/refund", content, cancellationToken);
        if(result.IsSuccessStatusCode == false)
            throw new ApplicationException("Error creating refund request");
        
        var totalRefunded = await result.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<decimal>(totalRefunded);
    }
}