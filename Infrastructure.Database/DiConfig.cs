using Core.Refund;
using Infrastructure.Database.Repository;
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
        
        services.AddTransient<IRefundProcessingRepository, RefundProcessingRepository>();
    }
}