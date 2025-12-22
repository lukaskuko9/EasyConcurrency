using EasyConcurrency.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Stubs;

namespace EasyConcurrency.IntegrationTests.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<MyLockableEntity> MyLockableEntities { get; init; }
    
    /// <inheritdoc />
    public DatabaseContext() : this(new DbContextOptions<DatabaseContext>())
    {
    }
    
    public static string GetConnectionString()
        => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? DatabaseContextFactory.DefaultConnectionString;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyLockableEntity>(entityBuilder =>
        {
            entityBuilder.ToTable("MyLockableEntities");
            entityBuilder.HasKey(refundEntity => refundEntity.Id);

            entityBuilder.Property(refundEntity => refundEntity.LockedUntil)
                .AddTimeLockConversion()
                .ValueGeneratedNever();
            
            entityBuilder.HasIndex(myDbEntity => myDbEntity.LockedUntil);
        });
    }
}
