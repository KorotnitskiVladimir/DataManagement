using Dapper;
using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DataManagement.ADO;

public class AdoDemo
{
    private SqlConnection? _sqlConnection;

    public SqlConnection sqlConnection
    {
        get
        {
            if (_sqlConnection == null)
            {
                string? connectionstring = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText("appsettings.json"))
                    .GetProperty("ConnectionStrings")
                    .GetProperty("DB")
                    .GetString();
                if (connectionstring == null)
                {
                    throw new FileNotFoundException("Connection string not found");
                }

                _sqlConnection = new(connectionstring);
                _sqlConnection.Open();
            }
            return _sqlConnection!;
        }
    }
    public void Run()
    {
        Console.WriteLine("ADO.NET Demo");
        try
        {
            Console.WriteLine("Connection OK " + sqlConnection.State);
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
        //CreateTables();
        //InsertData();
        TableReader();
        DapperDemo();

    }

    private void TableReader()
    {
        using (SqlCommand cmd = new())
        {
            cmd.Connection = sqlConnection;
            cmd.CommandText = "SELECT * FROM Chart";
            // Выполнение команд с результатом - возвращает Reader
            using SqlDataReader reader = cmd.ExecuteReader();
            // Reader читает данные по одной строке, полная работа - в цикле
            while (reader.Read()) // Read - и читает и проверяет наличие данных
            {
                Console.WriteLine("Id - {0}, User - {1}, Content: {2}",
                    reader.GetString("Id"), reader["UserId"], reader["Products"]);
                // пока есть открытый DataReader нельзя открыть еще один
                // Например, это ограничивает запросы в середине цикла по другому запросу
            }
        }

        Console.WriteLine("------------------");

        // ORM
        // "SELECT TOP 1 * FROM UserRole" => new UserRole
        using (SqlCommand cmd2 = new())
        {
            cmd2.Connection = sqlConnection;
            cmd2.CommandText = "SELECT TOP 1 * FROM UserRole";
            using SqlDataReader reader2 = cmd2.ExecuteReader();
            reader2.Read();
            UserRole ur = new()
            {
                Id = reader2.GetString("Id"),
                Description = reader2.GetString("Description"),
                CanCreate = reader2.GetInt32("CanCreate"),
                CanRead = reader2.GetInt32("CanRead"),
                CanUpdate = reader2.GetInt32("CanUpdate"),
                CanDelete = reader2.GetInt32("CanDelete")
            };
            Console.WriteLine(ur);
        }
    }

    private void DapperDemo()
    {
        Console.WriteLine("---------DAPPER---------");
        // скалярные запросы - запросы на одне значение
        string sql = "SELECT COUNT(*) FROM UserRole";
        var cnt = sqlConnection.ExecuteScalar(sql);
        sql = "SELECT CURRENT_TIMESTAMP";
        DateTime dt = sqlConnection.ExecuteScalar<DateTime>(sql);
        Console.WriteLine($"in DB there are {cnt} roles at {dt}");
        
        // Одна строка данных
        // а) результат один
        UserRole ur = sqlConnection.QuerySingle<UserRole>("SELECT TOP 1 * FROM UserRole");
        Console.WriteLine(ur);
        // б) результатов несколько, но нужен только первый
        ur = sqlConnection.QueryFirst<UserRole>("SELECT TOP 2 * FROM UserRole");
        Console.WriteLine(ur);
        // в) с потенциальной возможностью отстутствия данных
        var urn = sqlConnection.QueryFirstOrDefault<UserRole>("SELECT * FROM UserRole WHERE Id = 'undefined'");
        Console.WriteLine(urn?.ToString() ?? "No data");
        
        // Несколько строк данных
        var roles = sqlConnection.Query<UserRole>("SELECT * FROM UserRole");
        // !! сам запрос (Query) возвращает IEnumerable, что не является коллекцией, а всего лишь ее генератором. Для
        // получения коллекции можно использовать метод .ToList(), но это может привести к задержкам и зависаниям при
        // наличии большого количества данных
        foreach (UserRole r in roles)
        {
            Console.WriteLine(r);
        }
        Console.WriteLine("-------------------");
        
        // Передача параметров в команды
        Console.WriteLine(sqlConnection.QuerySingleOrDefault<UserRole>("SELECT * FROM UserRole WHERE Id = @RoleId",
            new { RoleId = "moderator" }));
        
        Console.WriteLine("-----------------------");
        
        foreach (UserRole r in sqlConnection.Query<UserRole>("SELECT * FROM UserRole WHERE Id IN @RoleIds",
                     new { RoleIds = new string[] { "moderator", "guest" } }))
        {
            Console.WriteLine(r);
        }
        
        Console.WriteLine("----------------------");

        foreach (UserRole r in sqlConnection.Query<UserRole>(
                     "SELECT * FROM UserRole WHERE CanRead = @read AND CanUpdate = @update",
                     new {read = 1, update = 1}))
        {
            Console.WriteLine(r);
        }
    }

    private void InsertData()
    {
        try
        {
            /* Передача параметров в запрос
             * Актуально когда данные для запроса передаются отдельно - вводятся пользователем или поступают с сайтов
             * Используются для предотвращения SQL-инъекций - искажения SQL-запросов из-за вставки в них некоректных данных
             * (... VALUES('guest', '{descr}', ...) descr = "Подключенный пользователь"
             *
             * общий совет - разделять команду и данные:
             * вместо данных в SQL тексте подставляем параметры (@name)
             * а в объекте-команде заполняем коллекцию .Parameters
             */
            using SqlCommand cmd = new();
            cmd.Connection = sqlConnection;
            cmd.CommandText = "INSERT INTO Orders VALUES('OR-2', 'Chart-2')";
            //var param = cmd.Parameters.Add("description", SqlDbType.Text, 256);
            //param.Value = "User that can correct, but can't create";
            //var param2 = cmd.Parameters.Add("canCreate", SqlDbType.Int);
            //param2.Value = 0;
            cmd.ExecuteNonQuery();
            Console.WriteLine("SQL OK");
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private void QueryDdl(string sql)
    {
        using SqlCommand cmd = new(sql, sqlConnection);
        //cmd.Connection = sqlConnection;
        //cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private void CreateTables()
    {
        try
        {
            QueryDdl("CREATE TABLE UserData (" + "Id UNIQUEIDENTIFIER PRIMARY KEY," +
                     "Name NVARCHAR(128) NOT NULL," +
                     "Email NVARCHAR(256) NOT NULL," +
                     "Phone VARCHAR(32) NULL)");
            Console.WriteLine("SQL OK");
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }

        try
        {
            QueryDdl("CREATE TABLE UserAccess (" + "Id UNIQUEIDENTIFIER PRIMARY KEY," +
                     "UserId UNIQUEIDENTIFIER NOT NULL," +
                     "RoleId UNIQUEIDENTIFIER NOT NULL," +
                     "Login NVARCHAR(256) NOT NULL," +
                     "Salt CHAR(16) NOT NULL," +
                     "Dk VARCHAR(32) NOT NULL)");
            Console.WriteLine("SQL OK");
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
        
        try
        {
            QueryDdl("CREATE TABLE UserRole (" + "Id VARCHAR(32) PRIMARY KEY," +
                     "Description NVARCHAR(256) NOT NULL," +
                     "CanCreate INT NOT NULL," +
                     "CanRead INT NOT NULL," +
                     "CanUpdate INT NOT NULL, " +
                     "CanDelete INT NOT NULL)");
            Console.WriteLine("SQL OK");
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
        
        try
        {
            QueryDdl("CREATE TABLE Goods (" + "Id VARCHAR(64) PRIMARY KEY," +
                     "Description NVARCHAR(256) NOT NULL," +
                     "Producer NVARCHAR(256) NOT NULL, " +
                     "Price DOUBLE PRECISION NOT NULL)");
            Console.WriteLine("SQL OK");
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
        
        try
        {
            QueryDdl("CREATE TABLE Chart (" + "Id VARCHAR(32) PRIMARY KEY," +
                     "UserId VARCHAR(32) NULL," +
                     "Products NVARCHAR(256) NULL)");
            Console.WriteLine("SQL OK");
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
        
        try
        {
            QueryDdl("CREATE TABLE Orders (" + "Id VARCHAR(32) PRIMARY KEY," +
                     "ChartId VARCHAR(32) NULL)");
            Console.WriteLine("SQL OK");
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}
// 0. Проверить блок Data Storage and Processing
// 1. NuGet - система управления пакетами
//      Через NuGet устанавливаем Microsoft Data SqlClient - имплементация ADO для MS SQL Server
// 2. Создаем БД: или / или: LocalDB или Management Studio
// 3. Узнаем строку подключения
// data source=DESKTOP-BGOKU7C\SQLEXPRESS;initial catalog=master;trusted_connection=true
// data source=DESKTOP-BGOKU7C\SQLEXPRESS;initial catalog=DataManagement;trusted_connection=true
// 4. Подключаемся, тестируем подключение

/*
use DataManagement;
select
    'data source=' + @@servername +
    ';initial catalog=' + db_name() +
    case type_desc
        when 'WINDOWS_LOGIN' 
            then ';trusted_connection=true'
        else
            ';user id=' + suser_name() + ';password=<<YourPassword>>'
    end
    as ConnectionString
from sys.server_principals
where name = suser_name()
*/

/*
 * Общая схема работы
 * 0. Сохранение строки подключения. Рекомендуется в файлы настроек (appsettings.json)
 * 1. Подключение и сохранение его на максимальное время (на время работы программы)
 * 2. Выполнение запросов
 *  2.1. Без возврата результатов
 *  2.2. С результатами
*/