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
    public DbSet<PickingTasks> PickingTasks { get; set; }
    public DbSet<PickingItems> PickingItems { get; set; }
    public DbSet<PackingTasks> PackingTasks { get; set; }
    public DbSet<PackingItems> PackingItems { get; set; }
    public DbSet<Shipments> Shipments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
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