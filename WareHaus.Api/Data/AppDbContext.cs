using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Models;

namespace WareHaus.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Products> Products { get; set; }
    public DbSet<PurchaseOrders> PurchaseOrders { get; set; }
    public DbSet<POItems> POItems { get; set; }
    public DbSet<ReceivingLogs> ReceivingLogs { get; set; }
    public DbSet<Zones> Zones { get; set; }
    public DbSet<Shelves> Shelves { get; set; }
    public DbSet<Stocks> Stocks { get; set; }

    public DbSet<SalesOrders> SalesOrders { get; set; }
    public DbSet<SOItems> SOItems { get; set; }
    public DbSet<PackingLogs> PackingLogs { get; set; }
    public DbSet<ShippingLogs> ShippingLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureInboundRelations(modelBuilder);
        ConfigureOutboundRelations(modelBuilder);
    }

    private static void ConfigureInboundRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrders>()
            .HasMany(po => po.POItems)
            .WithOne(item => item.PurchaseOrder)
            .HasForeignKey(item => item.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<POItems>()
            .HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReceivingLogs>()
            .HasOne(log => log.POItem)
            .WithMany()
            .HasForeignKey(log => log.POItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Stocks>()
            .HasOne(stock => stock.Product)
            .WithMany()
            .HasForeignKey(stock => stock.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Stocks>()
            .HasOne(stock => stock.Shelf)
            .WithMany()
            .HasForeignKey(stock => stock.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureOutboundRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesOrders>(entity =>
        {
            entity.HasKey(so => so.Id);

            entity.Property(so => so.SONumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(so => so.CustomerName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(so => so.Status)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(so => so.SONumber)
                .IsUnique();

            entity.HasQueryFilter(so => so.DeletedAt == null);
        });

        modelBuilder.Entity<SOItems>(entity =>
        {
            entity.HasKey(item => item.Id);

            entity.Property(item => item.Quantity)
                .IsRequired();

            entity.Property(item => item.PackedQuantity)
                .IsRequired();

            entity.HasOne(item => item.SalesOrder)
                .WithMany(so => so.SOItems)
                .HasForeignKey(item => item.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(item => item.DeletedAt == null);
        });

        modelBuilder.Entity<PackingLogs>(entity =>
        {
            entity.HasKey(log => log.Id);

            entity.Property(log => log.PackedBy)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(log => log.QuantityPacked)
                .IsRequired();

            entity.HasOne(log => log.SalesOrder)
                .WithMany(so => so.PackingLogs)
                .HasForeignKey(log => log.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(log => log.Product)
                .WithMany()
                .HasForeignKey(log => log.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(log => log.DeletedAt == null);
        });

        modelBuilder.Entity<ShippingLogs>(entity =>
        {
            entity.HasKey(log => log.Id);

            entity.Property(log => log.CourierName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(log => log.TrackingNumber)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(log => log.ShippingStatus)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(log => log.SalesOrder)
                .WithMany(so => so.ShippingLogs)
                .HasForeignKey(log => log.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(log => log.DeletedAt == null);
        });
    }

    public override int SaveChanges()
    {
        HandleAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void HandleAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(entry =>
                entry.Entity.GetType().GetProperty("CreatedAt") != null &&
                (
                    entry.State == EntityState.Added ||
                    entry.State == EntityState.Modified ||
                    entry.State == EntityState.Deleted
                ));

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = now;
                entry.Property("UpdatedAt").CurrentValue = null;
                entry.Property("DeletedAt").CurrentValue = null;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = now;
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Property("DeletedAt").CurrentValue = now;
            }
        }
    }
}