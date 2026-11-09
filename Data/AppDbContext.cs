using System.IO;
using Microsoft.EntityFrameworkCore;
using SmallDatabaseApp.Models;

namespace SmallDatabaseApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Person> People => Set<Person>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var databasePath = Path.Combine(AppContext.BaseDirectory, "people.db");
        optionsBuilder.UseSqlite($"Data Source={databasePath}");
    }
}
