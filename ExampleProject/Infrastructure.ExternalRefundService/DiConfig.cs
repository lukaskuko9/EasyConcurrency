﻿using Core.ExternalRefundService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ExternalRefundService;

public static class DiConfig
{
    public static void RegisterExternalRefundServiceClient(IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IExternalRefundServiceClient, ExternalRefundServiceClient>();

        services.AddOptions<ExternalRefundServiceOptions>(nameof(ExternalRefundServiceOptions));
        services.Configure<ExternalRefundServiceOptions>(
            config.GetSection(nameof(ExternalRefundServiceOptions))
        );
    }
}