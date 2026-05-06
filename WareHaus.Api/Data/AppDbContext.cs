using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Models;

namespace WareHaus.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
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
    public DbSet<PackingTasks> PackingTasks { get; set; }
    public DbSet<PackingItems> PackingItems { get; set; }
    public DbSet<Shipments> Shipments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================
        // Table
        // =====================
        modelBuilder.Entity<Products>().ToTable("Products");
        modelBuilder.Entity<PurchaseOrders>().ToTable("PurchaseOrders");
        modelBuilder.Entity<POItems>().ToTable("POItems");
        modelBuilder.Entity<ReceivingLogs>().ToTable("ReceivingLogs");

        modelBuilder.Entity<Zones>().ToTable("Zones");
        modelBuilder.Entity<Shelves>().ToTable("Shelves");
        modelBuilder.Entity<Stocks>().ToTable("Stocks");

        modelBuilder.Entity<SalesOrders>().ToTable("SalesOrders");
        modelBuilder.Entity<SOItems>().ToTable("SOItems");
        modelBuilder.Entity<PackingTasks>().ToTable("PackingTasks");
        modelBuilder.Entity<PackingItems>().ToTable("PackingItems");
        modelBuilder.Entity<Shipments>().ToTable("Shipments");

        // =====================
        // Primary Key
        // =====================
        modelBuilder.Entity<Products>().HasKey(product => product.Id);
        modelBuilder.Entity<PurchaseOrders>().HasKey(purchaseOrder => purchaseOrder.Id);
        modelBuilder.Entity<POItems>().HasKey(poItem => poItem.Id);
        modelBuilder.Entity<ReceivingLogs>().HasKey(receivingLog => receivingLog.Id);

        modelBuilder.Entity<Zones>().HasKey(zone => zone.Id);
        modelBuilder.Entity<Shelves>().HasKey(shelf => shelf.Id);
        modelBuilder.Entity<Stocks>().HasKey(stock => stock.Id);

        modelBuilder.Entity<SalesOrders>().HasKey(salesOrder => salesOrder.Id);
        modelBuilder.Entity<SOItems>().HasKey(soItem => soItem.Id);
        modelBuilder.Entity<PackingTasks>().HasKey(packingTask => packingTask.Id);
        modelBuilder.Entity<PackingItems>().HasKey(packingItem => packingItem.Id);
        modelBuilder.Entity<Shipments>().HasKey(shipment => shipment.Id);

        // =====================
        // Product
        // =====================
        modelBuilder.Entity<Products>(entity =>
        {
            entity.Property(product => product.SKU)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(product => product.ProductName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(product => product.Barcode)
                .HasMaxLength(100);

            entity.Property(product => product.UnitOfMeasure)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(product => product.SKU)
                .IsUnique();

            entity.HasIndex(product => product.Barcode)
                .IsUnique();
        });

        // =====================
        // Zone
        // =====================
        modelBuilder.Entity<Zones>(entity =>
        {
            entity.Property(zone => zone.ZoneCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(zone => zone.ZoneName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(zone => zone.Description)
                .HasMaxLength(255);

            entity.Property(zone => zone.Category)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(zone => zone.TotalAisle)
                .IsRequired();

            entity.Property(zone => zone.ShelfPerAisle)
                .IsRequired();

            entity.Property(zone => zone.LevelPerShelf)
                .IsRequired();

            entity.HasIndex(zone => zone.ZoneCode)
                .IsUnique();
        });

        // =====================
        // Shelf
        // =====================
        modelBuilder.Entity<Shelves>(entity =>
        {
            entity.Property(shelf => shelf.ZoneId)
                .IsRequired();

            entity.Property(shelf => shelf.Aisle)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(shelf => shelf.ShelfCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(shelf => shelf.Capacity)
                .IsRequired();

            entity.Property(shelf => shelf.CurrentVolume)
                .IsRequired();

            entity.Property(shelf => shelf.QRCodePath)
                .HasMaxLength(255);

            entity.HasIndex(shelf => shelf.ShelfCode)
                .IsUnique();
        });

        // =====================
        // Purchase Order
        // =====================
        modelBuilder.Entity<PurchaseOrders>(entity =>
        {
            entity.Property(purchaseOrder => purchaseOrder.PONumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(purchaseOrder => purchaseOrder.SupplierName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(purchaseOrder => purchaseOrder.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(purchaseOrder => purchaseOrder.OrderDate)
                .IsRequired();

            entity.HasIndex(purchaseOrder => purchaseOrder.PONumber)
                .IsUnique();
        });

        // =====================
        // PO Item
        // =====================
        modelBuilder.Entity<POItems>(entity =>
        {
            entity.Property(poItem => poItem.PurchaseOrderId)
                .IsRequired();

            entity.Property(poItem => poItem.ProductId)
                .IsRequired();

            entity.Property(poItem => poItem.QtyExpected)
                .IsRequired();

            entity.Property(poItem => poItem.QtyReceived)
                .IsRequired();
        });

        // =====================
        // Receiving Log
        // =====================
        modelBuilder.Entity<ReceivingLogs>(entity =>
        {
            entity.Property(receivingLog => receivingLog.PurchaseOrderId)
                .IsRequired();

            entity.Property(receivingLog => receivingLog.POItemId)
                .IsRequired();

            entity.Property(receivingLog => receivingLog.ProductId)
                .IsRequired();

            entity.Property(receivingLog => receivingLog.QtyReceived)
                .IsRequired();

            entity.Property(receivingLog => receivingLog.Condition)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(receivingLog => receivingLog.ReceivedAt)
                .IsRequired();
        });

        // =====================
        // Stock
        // =====================
        modelBuilder.Entity<Stocks>(entity =>
        {
            entity.Property(stock => stock.ProductId)
                .IsRequired();

            entity.Property(stock => stock.ShelfId)
                .IsRequired();

            entity.Property(stock => stock.Quantity)
                .IsRequired();

            entity.HasIndex(stock => new { stock.ProductId, stock.ShelfId })
                .IsUnique();
        });

        // =====================
        // Sales Order
        // =====================
        modelBuilder.Entity<SalesOrders>(entity =>
        {
            entity.Property(salesOrder => salesOrder.SONumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(salesOrder => salesOrder.CustomerName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(salesOrder => salesOrder.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(salesOrder => salesOrder.OrderDate)
                .IsRequired();

            entity.HasIndex(salesOrder => salesOrder.SONumber)
                .IsUnique();
        });

        // =====================
        // SO Item
        // =====================
        modelBuilder.Entity<SOItems>(entity =>
        {
            entity.Property(soItem => soItem.SOId)
                .IsRequired();

            entity.Property(soItem => soItem.ProductId)
                .IsRequired();

            entity.Property(soItem => soItem.QtyOrdered)
                .IsRequired();

            entity.Property(soItem => soItem.QtyPicked)
                .IsRequired();
        });

        // =====================
        // Packing Task
        // =====================
        modelBuilder.Entity<PackingTasks>(entity =>
        {
            entity.Property(packingTask => packingTask.SOId)
                .IsRequired();

            entity.Property(packingTask => packingTask.TotalPackage)
                .IsRequired();

            entity.Property(packingTask => packingTask.PackingStatus)
                .HasMaxLength(30)
                .IsRequired();
        });

        // =====================
        // Packing Item
        // =====================
        modelBuilder.Entity<PackingItems>(entity =>
        {
            entity.Property(packingItem => packingItem.PackingTaskId)
                .IsRequired();

            entity.Property(packingItem => packingItem.ProductId)
                .IsRequired();

            entity.Property(packingItem => packingItem.QtyVerified)
                .IsRequired();
        });

        // =====================
        // Shipment
        // =====================
        modelBuilder.Entity<Shipments>(entity =>
        {
            entity.Property(shipment => shipment.PackingTaskId)
                .IsRequired();

            entity.Property(shipment => shipment.CourierName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(shipment => shipment.TrackingNumber)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(shipment => shipment.ShippingLabelUrl)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(shipment => shipment.Status)
                .HasMaxLength(30)
                .IsRequired();
        });

        // =====================
        // Relationships - Inbound
        // =====================
        modelBuilder.Entity<PurchaseOrders>()
            .HasMany(purchaseOrder => purchaseOrder.POItems)
            .WithOne(poItem => poItem.PurchaseOrders)
            .HasForeignKey(poItem => poItem.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchaseOrders>()
            .HasMany(purchaseOrder => purchaseOrder.ReceivingLogs)
            .WithOne(receivingLog => receivingLog.PurchaseOrders)
            .HasForeignKey(receivingLog => receivingLog.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Products>()
            .HasMany(product => product.POItems)
            .WithOne(poItem => poItem.Products)
            .HasForeignKey(poItem => poItem.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<POItems>()
            .HasMany(poItem => poItem.ReceivingLogs)
            .WithOne(receivingLog => receivingLog.POItems)
            .HasForeignKey(receivingLog => receivingLog.POItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Products>()
            .HasMany(product => product.Stocks)
            .WithOne(stock => stock.Products)
            .HasForeignKey(stock => stock.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Products>()
            .HasMany(product => product.PackingItems)
            .WithOne(packingItem => packingItem.Products)
            .HasForeignKey(packingItem => packingItem.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Zones>()
            .HasMany(zone => zone.Shelves)
            .WithOne(shelf => shelf.Zones)
            .HasForeignKey(shelf => shelf.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shelves>()
            .HasMany(shelf => shelf.Stocks)
            .WithOne(stock => stock.Shelves)
            .HasForeignKey(stock => stock.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================
        // Relationships - Outbound
        // =====================
        modelBuilder.Entity<SalesOrders>()
            .HasMany(salesOrder => salesOrder.SOItems)
            .WithOne(soItem => soItem.SalesOrders)
            .HasForeignKey(soItem => soItem.SOId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SalesOrders>()
            .HasMany(salesOrder => salesOrder.PackingTasks)
            .WithOne(packingTask => packingTask.SalesOrders)
            .HasForeignKey(packingTask => packingTask.SOId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Products>()
            .HasMany(product => product.SOItems)
            .WithOne(soItem => soItem.Products)
            .HasForeignKey(soItem => soItem.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PackingTasks>()
            .HasMany(packingTask => packingTask.PackingItems)
            .WithOne(packingItem => packingItem.PackingTasks)
            .HasForeignKey(packingItem => packingItem.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PackingTasks>()
            .HasMany(packingTask => packingTask.Shipments)
            .WithOne(shipment => shipment.PackingTasks)
            .HasForeignKey(shipment => shipment.PackingTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================
        // Query Filter - Soft Delete
        // =====================
        modelBuilder.Entity<Products>().HasQueryFilter(product => product.DeletedAt == null);
        modelBuilder.Entity<PurchaseOrders>().HasQueryFilter(purchaseOrder => purchaseOrder.DeletedAt == null);
        modelBuilder.Entity<POItems>().HasQueryFilter(poItem => poItem.DeletedAt == null);
        modelBuilder.Entity<ReceivingLogs>().HasQueryFilter(receivingLog => receivingLog.DeletedAt == null);

        modelBuilder.Entity<Zones>().HasQueryFilter(zone => zone.DeletedAt == null);
        modelBuilder.Entity<Shelves>().HasQueryFilter(shelf => shelf.DeletedAt == null);
        modelBuilder.Entity<Stocks>().HasQueryFilter(stock => stock.DeletedAt == null);

        modelBuilder.Entity<SalesOrders>().HasQueryFilter(salesOrder => salesOrder.DeletedAt == null);
        modelBuilder.Entity<SOItems>().HasQueryFilter(soItem => soItem.DeletedAt == null);
        modelBuilder.Entity<PackingTasks>().HasQueryFilter(packingTask => packingTask.DeletedAt == null);
        modelBuilder.Entity<PackingItems>().HasQueryFilter(packingItem => packingItem.DeletedAt == null);
        modelBuilder.Entity<Shipments>().HasQueryFilter(shipment => shipment.DeletedAt == null);

        // =====================
        // Data Seeding
        // =====================
        modelBuilder.Entity<Products>().HasData(
            new Products
            {
                Id = 1,
                SKU = "PRD-001",
                ProductName = "Sample Product",
                Barcode = "899000000001",
                UnitOfMeasure = "pcs",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DeletedAt = null
            }
        );

        modelBuilder.Entity<Zones>().HasData(
            new Zones
            {
                Id = 1,
                ZoneCode = "ZONE-A",
                ZoneName = "Zone A",
                Description = "Sample zone untuk testing",
                Category = "General",
                TotalAisle = 1,
                ShelfPerAisle = 1,
                LevelPerShelf = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DeletedAt = null
            }
        );

        modelBuilder.Entity<Shelves>().HasData(
            new Shelves
            {
                Id = 1,
                ZoneId = 1,
                Aisle = "A1",
                ShelfCode = "SH-A1-001",
                Capacity = 100,
                CurrentVolume = 0,
                QRCodePath = null,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DeletedAt = null
            }
        );
    }

    private void ApplyChanges()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntities>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = entry.Entity.CreatedAt == default
                    ? now
                    : entry.Entity.CreatedAt;

                entry.Entity.UpdatedAt = entry.Entity.UpdatedAt == default
                    ? now
                    : entry.Entity.UpdatedAt;

                entry.Entity.DeletedAt = null;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.Entity.DeletedAt = now;
                entry.Entity.UpdatedAt = now;

                entry.State = EntityState.Modified;
            }
        }
    }

    public override int SaveChanges()
    {
        ApplyChanges();

        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyChanges();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyChanges();

        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyChanges();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}