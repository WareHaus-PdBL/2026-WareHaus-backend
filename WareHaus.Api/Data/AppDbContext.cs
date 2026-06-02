using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Models;

namespace WareHaus.Api.Data;

public class AppDbContext : DbContext
{
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
}