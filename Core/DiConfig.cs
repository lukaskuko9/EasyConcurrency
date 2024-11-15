using Core.Refund;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DiConfig
{
    public static void RegisterDi(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRefundService, RefundService>();
    }
}