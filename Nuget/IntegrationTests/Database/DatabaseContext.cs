using EasyConcurrency.EntityFramework.Extensions;
using EasyConcurrency.EntityFramework.ValueConverter;
using EasyConcurrency.Tests.Shared;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.Tests.IntegrationTests.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<HasTimeLockEntity> HasTimeLockEntities { get; init; }
    public DbSet<CustomTimeLockEntity> CustomTimeLockEntities { get; init; }
    
    /// <inheritdoc />
    public DatabaseContext() : this(new DbContextOptions<DatabaseContext>())
    {
    }
    
    public static string GetConnectionString()
        => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? DatabaseContextFactory.DefaultConnectionString;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HasTimeLockEntity>(entityBuilder =>
        {
            entityBuilder.HasKey(refundEntity => refundEntity.Id);

            entityBuilder.Property(refundEntity => refundEntity.LockedUntil)
                .AddTimeLockConversion()
                .ValueGeneratedNever();
            
            entityBuilder.HasIndex(myDbEntity => myDbEntity.LockedUntil);
        });
        
        modelBuilder.Entity<CustomTimeLockEntity>(entityBuilder =>
        {
            entityBuilder.HasKey(refundEntity => refundEntity.Id);

            entityBuilder.Property(refundEntity => refundEntity.LockedUntil)
                .HasConversion(
                    timeLock => timeLock == null ? null : timeLock.Value.Value,
                    dateTimeOffset => new CustomTimeLock { Value = dateTimeOffset }
                )
                .IsConcurrencyToken();
            
            entityBuilder.HasIndex(myDbEntity => myDbEntity.LockedUntil);
        });
    }
}
