using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EasyConcurrency.Tests.IntegrationTests.Database;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public const string DefaultConnectionString = "Data Source=.;Initial Catalog=EFConcurrencyTests;Integrated Security=True;TrustServerCertificate=True";

    public DatabaseContext CreateDbContext(string[] args)
    {
        var connectionString = args.Length != 0 ? args[0] : DatabaseContext.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DatabaseContext(optionsBuilder.Options);
    }
}