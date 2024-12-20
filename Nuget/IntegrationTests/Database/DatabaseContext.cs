using EasyConcurrency.EntityFramework.TimeLock;
using Microsoft.EntityFrameworkCore;
using Stubs;

namespace IntegrationTests.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<MyDbConcurrentEntity> MyDbEntities { get; init; }
    public DbSet<MyLockableEntity> MyLockableEntities { get; init; }
    
    /// <inheritdoc />
    public DatabaseContext() : this(new DbContextOptions<DatabaseContext>())
    {
    }
    
    public static string GetConnectionString()
        => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? DatabaseContextFactory.DefaultConnectionString;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyDbConcurrentEntity>(entityBuilder =>
        {
            entityBuilder.ToTable("MyDbEntities");
            entityBuilder.HasKey(refundEntity => refundEntity.Id);

            entityBuilder.Property(refundEntity => refundEntity.LockedUntil).AddTimeLockConversion();
            
            entityBuilder.HasIndex(myDbEntity => myDbEntity.LockedUntil);
            entityBuilder.HasIndex(refundEntity => refundEntity.MyUniqueKey).IsUnique();
        });
        
        modelBuilder.Entity<MyLockableEntity>(entityBuilder =>
        {
            entityBuilder.ToTable("MyLockableEntities");
            entityBuilder.HasKey(refundEntity => refundEntity.Id);

            entityBuilder.Property(refundEntity => refundEntity.LockedUntil).AddTimeLockConversion();
            
            entityBuilder.HasIndex(myDbEntity => myDbEntity.LockedUntil);
        });
    }
}
