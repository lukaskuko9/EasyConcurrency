using EntityFrameworkCore.PessimisticConcurrency;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<MyDbEntity> MyDbEntities { get; set; }
    
    /// <inheritdoc />
    public DatabaseContext() : this(new DbContextOptions<DatabaseContext>())
    {
    }
    
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