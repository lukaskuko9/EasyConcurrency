using Core.PrimitiveTypeObsession;
using Infrastructure.Database.Entities;
using Infrastructure.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Infrastructure.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<RefundEntity> Refunds { get; set; }

    /// <inheritdoc />
    public DatabaseContext() : this(new DbContextOptions<DatabaseContext>())
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefundEntity>(b =>
        {
            b.ToTable("Refunds");
            
            b.HasKey(refundEntity => refundEntity.Id);
            
            b.Property(refundEntity => refundEntity.Id)
                .HasConversion(
                    dbId => dbId.Value,
                    longId => new DatabaseId(longId)
                )
                .UseIdentityColumn();
            
            b.Property(refundEntity => refundEntity.LockedUntil)
                .HasConversion(
                    timeLock => timeLock == null ? null : timeLock.Value.Value,
                    dateTimeOffset => new TimeLock(dateTimeOffset)
                )
                .IsRequired(false);

           // b.HasQueryFilter(x=>x.LockedUntil == null || x.LockedUntil < DateTimeOffset.Now);
            b.Property(refundEntity=>refundEntity.Amount).HasColumnType("decimal(18,2)");
            
            b.HasIndex(refundEntity => refundEntity.AccountKey).IsUnique();
            b.HasIndex(refundEntity => new { refundEntity.LockedUntil, refundEntity.IsProcessed });
        });
    }
}