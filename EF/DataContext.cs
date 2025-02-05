using DataManagement.EF.Entities;
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

    public DataContext() : base() {}

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
        // Сидирование (seeding) - включение изначальных данных.
        modelBuilder.Entity<Entities.UserData>().HasData(
            new Entities.UserData() { 
                Id = Guid.Parse("3C6659D1-7012-4064-844A-5F3EFDADDA17"), 
                Email = "user1@gmail.com", Name = "Пользователь 1", Phone = "+555-11" },
            new Entities.UserData()
            {
                Id = Guid.Parse("1F4FE942-54D9-417D-8F35-111B2A37530C"), 
                Email = "user2@gmail.com", Name = "Пользователь 2", Phone = "+555-22"
            },
            new Entities.UserData()
            {
                Id = Guid.Parse("C3413E3E-312D-4C8C-B33B-783EC882F9C0"), 
                Email = "user3@gmail.com", Name = "Пользователь 3", Phone = "+555-33"
            },
            new Entities.UserData()
            {
                Id = Guid.Parse("96ED09D3-90BE-4D20-A5B3-1A60DA83C28C"), 
                Email = "user4@gmail.com", Name = "Пользователь 4", Phone = "+555-44"
            },
            new Entities.UserData()
            {
                Id = Guid.Parse("1F0216AA-4280-4B92-A03F-F8421724D639"), 
                Email = "user5@gmail.com", Name = "Пользователь 5", Phone = "+555-55"
            },
            new Entities.UserData()
            {
                Id = Guid.Parse("439029C2-D2A2-43B6-8613-60AD5CAF6C4A"),
                Email = "user6@gmail.com", Name = "Jane Dow", Phone = "+222-11"
            },
            new Entities.UserData()
            {
                Id = Guid.Parse("2C9073DA-0C78-40B1-BBAA-C10E62CD01EF"),
                Email = "user7@gmail.com", Name = "Alex Wong", Phone = "+222-33"
            },
            new Entities.UserData()
            {
                Id = Guid.Parse("AD7AB337-96DB-4ADF-B908-42D4A4F8B96A"),
                Email = "user8@gmail.com", Name = "Karan Dash", Phone = "+222-55"
            },
            new Entities.UserData()
            {
                Id = Guid.Parse("C26BA61A-4039-4E6C-B212-0152E1046644"),
                Email = "user9@gmail.com", Name = "Lloyce Lane", Phone = "+333-11"
            });
        
        modelBuilder.Entity<Entities.UserRole>().HasData(
            new Entities.UserRole()
            {
                Id = "guest", Description = "solely registered user", CanCreate = 0, CanRead = 0, CanUpdate = 0,
                CanDelete = 0
            },
            new Entities.UserRole()
            {
                Id = "editor", Description = "has authority to edit content", CanCreate = 0, CanRead = 1, CanUpdate = 1,
                CanDelete = 0
            },
            new Entities.UserRole()
                { Id = "admin", Description = "admin of DB", CanCreate = 1, CanRead = 1, CanUpdate = 1, CanDelete = 1 },
            new Entities.UserRole()
            {
                Id = "moderator", Description = "has authority to block", CanCreate = 0, CanRead = 1, CanUpdate = 0,
                CanDelete = 1
            });
        string salt = "12345";
        string defaultPassword = "123";
        modelBuilder.Entity<Entities.UserAccess>().HasData(
            new Entities.UserAccess()
            {
                Id = Guid.Parse("46B67F0F-B5E6-4BF1-9FC6-83FEB1010B62"),
                UserId = Guid.Parse("3C6659D1-7012-4064-844A-5F3EFDADDA17"),
                RoleId = "guest", Login = "user1", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("120A0620-5297-4FD4-B90D-CC736F9BF8C2"),
                UserId = Guid.Parse("3C6659D1-7012-4064-844A-5F3EFDADDA17"),
                RoleId = "moderator", Login = "user1-m", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("AB395C0B-188A-45D5-88CD-44097BCB6038"),
                UserId = Guid.Parse("C3413E3E-312D-4C8C-B33B-783EC882F9C0"),
                RoleId = "guest", Login = "user2", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("2950C25A-2B62-4FE3-8BBA-45C98C2D4FB5"),
                UserId = Guid.Parse("1F4FE942-54D9-417D-8F35-111B2A37530C"),
                RoleId = "guest", Login = "user4", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("CDED6DEF-3AD2-4B63-832D-BCAC35D8A014"),
                UserId = Guid.Parse("1F4FE942-54D9-417D-8F35-111B2A37530C"),
                RoleId = "guest", Login = "user5", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("C4DB1541-E406-4E45-9352-668E75945F7C"),
                UserId = Guid.Parse("1F4FE942-54D9-417D-8F35-111B2A37530C"),
                RoleId = "admin", Login = "user5-a", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("30000EE1-BC4E-43A5-92C2-B5BD230AC990"),
                UserId = Guid.Parse("1F4FE942-54D9-417D-8F35-111B2A37530C"),
                RoleId = "editor", Login = "user4-e", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("9D0621AF-A5BF-4E37-9DD0-3235F7BAA52B"),
                UserId = Guid.Parse("439029C2-D2A2-43B6-8613-60AD5CAF6C4A"),
                RoleId = "guest", Login = "user6-g", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("82E2E256-588C-4A10-827A-E91D09B188C6"),
                UserId = Guid.Parse("439029C2-D2A2-43B6-8613-60AD5CAF6C4A"),
                RoleId = "moderator", Login = "user6-m", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("96C05C4E-DF7B-45BE-A7F8-A1809552EF31"),
                UserId = Guid.Parse("2C9073DA-0C78-40B1-BBAA-C10E62CD01EF"),
                RoleId = "guest", Login = "user7-g", Salt = salt, Dk = salt + defaultPassword 
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("BD134EE3-B51B-467D-93CD-0F73B5974683"),
                UserId = Guid.Parse("AD7AB337-96DB-4ADF-B908-42D4A4F8B96A"),
                RoleId = "guest", Login = "user8-g", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("91B4C51F-149C-4ECF-A698-7E33A2295A5F"),
                UserId = Guid.Parse("AD7AB337-96DB-4ADF-B908-42D4A4F8B96A"),
                RoleId = "editor", Login = "user8-e", Salt = salt, Dk = salt + defaultPassword
            },
            new Entities.UserAccess()
            {
                Id = Guid.Parse("4EA41CDB-1592-4CED-AA0A-ADF2DE332EA2"),
                UserId = Guid.Parse("C26BA61A-4039-4E6C-B212-0152E1046644"),
                RoleId = "guest", Login = "user9-g", Salt = salt, Dk = salt + defaultPassword
            });
    }
}
