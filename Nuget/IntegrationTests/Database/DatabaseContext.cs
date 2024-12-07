using EasyConcurrency.EntityFramework;
using EasyConcurrency.EntityFramework.TimeLock;
using Microsoft.EntityFrameworkCore;
using Stubs;

namespace IntegrationTests.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<MyDbEntity> MyDbEntities { get; set; }
    
    /// <inheritdoc />
    public DatabaseContext() : this(new DbContextOptions<DatabaseContext>())
    {
    }
    
    public static string GetConnectionString()
    => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
       "Data Source=.;Initial Catalog=EFConcurrencyTests;Integrated Security=True;TrustServerCertificate=True";
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyDbEntity>(entityBuilder =>
        {
            entityBuilder.ToTable("MyDbEntities");
            entityBuilder.HasKey(refundEntity => refundEntity.Id);
            
            entityBuilder.Property(refundEntity => refundEntity.Version).IsRowVersion();
            entityBuilder.Property(refundEntity => refundEntity.LockedUntil).AddTimeLockConversion();
            
            entityBuilder.HasIndex(myDbEntity => myDbEntity.LockedUntil);
            entityBuilder.HasIndex(refundEntity => refundEntity.MyUniqueKey).IsUnique();
        });
    }
}