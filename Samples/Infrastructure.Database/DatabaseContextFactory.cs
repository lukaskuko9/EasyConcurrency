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
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine);

        return new DatabaseContext(optionsBuilder.Options);
    }
    
    public async Task CreateCleanDatabase(string[] args)
    {
        await using var dbContext = CreateDatabaseContext(args);

        //make sure we delete anything we previously stored so we can start from fresh
        await dbContext.Database.EnsureDeletedAsync();

        //ensure that database is created again
        await dbContext.Database.EnsureCreatedAsync();
    }

    public DatabaseContext CreateDatabaseContext(string[] args)
    {
        //create database context; for simplicity we create it from factory
        return CreateDbContext(args);
    }
}