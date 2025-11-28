using Microsoft.EntityFrameworkCore;
using FleetBook.Models;

namespace FleetBook.Data;
public class ApplicationDbContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<User> Users { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=fleetbook.db");
}