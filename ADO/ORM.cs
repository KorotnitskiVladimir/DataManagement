namespace DataManagement.ADO;

// "CREATE TABLE UserData (" + "Id UNIQUEIDENTIFIER PRIMARY KEY," +
// "Name NVARCHAR(128) NOT NULL," +
// "Email NVARCHAR(256) NOT NULL," +
// "Phone VARCHAR(32) NULL)"

public class UserData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}

// "CREATE TABLE UserAccess (" + "Id UNIQUEIDENTIFIER PRIMARY KEY," +
//    "UserId UNIQUEIDENTIFIER NOT NULL," +
//    "RoleId UNIQUEIDENTIFIER NOT NULL," +
//    "Login NVARCHAR(256) NOT NULL," +
//    "Salt CHAR(16) NOT NULL," +
//    "Dk VARCHAR(32) NOT NULL)"

public class UserAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RoleId { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public string Dk { get; set; } = null!;
}

// "CREATE TABLE UserRole (" + "Id VARCHAR(32) PRIMARY KEY," +
//    "Description NVARCHAR(256) NOT NULL," +
//    "CanCreate INT NOT NULL," +
//    "CanRead INT NOT NULL," +
//    "CanUpdate INT NOT NULL, " +
//    "CanDelete INT NOT NULL)"

public class UserRole
{
    public string Id { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CanCreate { get; set; }
    public int CanRead { get; set; }
    public int CanUpdate { get; set; }
    public int CanDelete { get; set; }

    public override string ToString()
    {
        return $"{Id} ({Description}): {CanCreate}/{CanRead}/{CanUpdate}/{CanDelete}";
    }
}

/*
 * ORM - Object Relation Mapping - отображение данных и их связей на объекты и их связи
 * Суть - данные из формата, в котором они передаются (DB, JSON, XML, CSV), превращаются в объекты языка программирования
 * (их коллекции), с которыми продолжается алгоритмическая работа
 * 
 *                                           ORM
 * { "Name": "The User". "Email": "u@i.ua" } => new User)("The User", "u@i.ua")
 *
 * class User {   | DTO - Data Transfer Object
 * String Name;   | Entity
 * String Email;  |
 * }
*/
