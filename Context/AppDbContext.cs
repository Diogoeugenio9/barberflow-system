using BarberFlow.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.API.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}