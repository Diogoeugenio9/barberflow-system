using BarberFlow.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.API.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Service> Services { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>()
            .Property(x => x.Price)
            .HasPrecision(10, 2);
    }
}