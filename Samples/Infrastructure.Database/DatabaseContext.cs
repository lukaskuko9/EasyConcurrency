using EasyConcurrency.EntityFramework.TimeLock;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<SampleEntity> SampleEntities { get; set; }
    
    public DatabaseContext() : this(new DbContextOptions<DatabaseContext>())
    {
    }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SampleEntity>(entityBuilder =>
        {
            entityBuilder.HasKey(sampleEntity => sampleEntity.Id);
            entityBuilder.Property(sampleEntity => sampleEntity.LockedUntil).AddTimeLockConversion();
            
            entityBuilder.HasIndex(sampleEntity => new {sampleEntity.IsProcessed, sampleEntity.LockedUntil});
            entityBuilder.HasIndex(sampleEntity => sampleEntity.MyUuid).IsUnique();
        });
    }
}