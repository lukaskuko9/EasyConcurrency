using Core.Refund;
using Infrastructure.Database.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database;

public static class DiConfig
{
    public static void RegisterDatabase(IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Database");
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        
        services.AddScoped<IRefundStorage, RefundStorage>();
    }
}