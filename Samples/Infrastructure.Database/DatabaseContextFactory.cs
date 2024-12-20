using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    private const string DefaultConnectionString = "Data Source=.;Initial Catalog=EasyConcurrencySamples;Integrated Security=True;TrustServerCertificate=True";

    public DatabaseContext CreateDbContext(string[] args)
    {
        var connectionString = args.Length != 0 ? args[0] : DefaultConnectionString;
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DatabaseContext(optionsBuilder.Options);
    }
}