using Microsoft.EntityFrameworkCore;

namespace DataManagement.EF;

public class DataContext : DbContext
{
    public DbSet<Entities.UserData> UsersData { get; private set; }
    public DbSet<Entities.UserRole> UserRoles { get; private set; }
    public DbSet<Entities.UserAccess> UserAccesses { get; private set; }
    public DbSet<Entities.Product> Products { get; private set; }
    public DbSet<Entities.Chart> Charts { get; private set; }
    public DbSet<Entities.Orders> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "data source=DESKTOP-BGOKU7C\\SQLEXPRESS;initial catalog=dm-ef;Integrated Security=SSPI;" +
            "TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Demo");
        modelBuilder.Entity<Entities.UserAccess>().HasIndex(a => a.Login).IsUnique();
    }
}