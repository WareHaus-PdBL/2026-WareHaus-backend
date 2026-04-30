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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}