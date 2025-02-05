using System.Diagnostics.SymbolStore;
using DataManagement.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace DataManagement.EF;

public class EFDemo
{
    public void Run()
    {
        DataContext dataContext = new();
        
        Console.WriteLine($"Database connected. Users: {dataContext.UsersData.Count()} \n" +
                          $"Roles: {dataContext.UserRoles.Count()} \n" +
                          $"Accesses: {dataContext.UserAccesses.Count()}");
        Console.WriteLine("All users:");
        foreach (var userData in dataContext.UsersData)
        {
            Console.WriteLine($"{userData.Name} ({userData.Email}) ({userData.Phone})");
        }
        Console.WriteLine("First 3 users:");
        var f3 = dataContext.UsersData.Take(3);
        // Выполнен ли запрос? сколько элементов в f3?
        // Вопрос не корректен
        // Запрос на данном этапе не выполнен. Сама инструкция f3 строит текст/алгоритм запроса, но не выполняет его.
        // И точно не формирует коллекци.
        foreach (var user in f3) // запрос (f3) запускается или циклом, или агрегатором (Count/Max/Sum..) или
        {                        // преобразователем (.ToList/.ToArray/...)
            Console.WriteLine($"{user.Name} ({user.Email})");
            //var ua = dataContext.UserAccesses.Where(ua => ua.UserId == user.Id);
            //foreach (var a in ua) // Connection уже имеет связанный открытый поставщик DataReader. Уже есть открытый
            //{                         // DataReader, который проходит по f3
            //    Console.WriteLine($"Access: {a.Login} ({a.RoleId})");
            //}
        }

        var udUA = dataContext.UsersData.Join(dataContext.UserAccesses, ud => ud.Id, ua => ua.UserId,
            (ud, ua) => $"{ud.Name} {ua.Login}");
        // SELECT * FROM Demo.UsersData as ud JOIN Demo.UserAccesses as ua ON ud.Id = ua.UserId
        Console.WriteLine("-------------- UserData + UserAccess -----------------");
        foreach (string str in udUA)
        {
            Console.WriteLine(str);
        }
        Console.WriteLine("--------Saving to objects-------");
        foreach (var pair in dataContext.UsersData.Join(
                     dataContext.UserAccesses, 
                     ud => ud.Id, ua => ua.UserId, 
                     (ud, ua) => new { ud, ua }))
        {
            Console.WriteLine($"{pair.ud.Email} {pair.ua.RoleId}");
        }
        
        // SELECT ud.Name, COUNT (UA.Login)
        // FROM Demo.UsersData as ud JOIN Demo.UserAccesses as ua ON ud.Id = ua.UserId
        // GROUP BY ud.Name
        
        Console.WriteLine("--------Group Join-------");
        foreach (var pair in dataContext.UsersData.GroupJoin(
                     dataContext.UserAccesses, 
                     ud => ud.Id, ua => ua.UserId, 
                     (ud, ua) => new { ud, cnt = ua.Count() }))
        {
            Console.WriteLine($"{pair.ud.Email} {pair.cnt}");
        }
        
        //SELECT ud.Name, ua.Login
        //    FROM Demo.UsersData as ud JOIN Demo.UserAccesses as ua ON ud.Id = ua.UserId
        //WHERE ua.RoleId = 'guest'
        
        Console.WriteLine("--------Using WHERE-------");
        foreach (var data in dataContext.UsersData.Join(
                         dataContext.UserAccesses,
                         ud => ud.Id, ua => ua.UserId,
                         (ud, ua) => new { ud, ua })
                     .Where(udua => udua.ua.RoleId == "guest")
                     .Select(udua => $"{udua.ud.Name} {udua.ua.Login} {udua.ua.RoleId}"))
        {
            Console.WriteLine(data);
        }
        
        // Вывести данные: имя пользователя - описание роли (description)
        
        var query1 = dataContext.UsersData.Join(
                         dataContext.UserAccesses,
                         ud => ud.Id, ua => ua.UserId, 
                         (ud, ua) => new {ud, ua});
        var query3 = query1.Join(dataContext.UserRoles,
            v => v.ua.RoleId, ur => ur.Id,
            (v, ur) => new { v, ur });
        foreach (var r in query3)
        {
            Console.WriteLine($"{r.v.ud.Name} {r.ur.Description}");
        }
        var query2 = dataContext.UserRoles.GroupJoin(
            dataContext.UserAccesses, ur => ur.Id, ua => ua.RoleId,
            (ur, uaGrp) => $"{ur.Description} {uaGrp.Count()} {String.Join(',', uaGrp.Select(
                a => a.Login))}");
        foreach (var a in query2)
        {
            Console.WriteLine(a);
        }
        Console.WriteLine("----Async----");
        dataContext.UsersData.Where(u => u.Name.Contains("о")).ForEachAsync(Console.WriteLine).Wait();
        dataContext.UsersData.Join(dataContext.UserAccesses,
            ud => ud.Id, ua => ua.UserId,
            (ud, ua) => new { ud, ua }).Join(dataContext.UserRoles,
            q => q.ua.RoleId, ur => ur.Id,
            (q, ur) => $"{q.ud.Name} {ur.Description}").ForEachAsync(Console.WriteLine).Wait();
        Console.WriteLine("----Task 1----");
        var canUpdate = dataContext.UserRoles.Join(dataContext.UserAccesses,
            ur => ur.Id, ua => ua.RoleId,
            (ur, ua) => new { ur, ua }).Where(urua => urua.ur.CanUpdate == 1).GroupJoin(
            dataContext.UsersData,
            urua => urua.ua.UserId, ud => ud.Id,
            (urua, ud) => $"{ud}");
        Console.WriteLine(canUpdate.Count());
        Console.WriteLine("------Task2-----\nLogins of users with authorization to update data");
        dataContext.UserAccesses.Join(dataContext.UserRoles,
                ua => ua.RoleId, ur => ur.Id,
                (ua, ur) => new { ua, ur}).Where(uaur => uaur.ur.CanUpdate == 1)
            .Select(uaur => uaur.ua.Login)
            .ForEachAsync(Console.WriteLine).Wait();
        Console.WriteLine("-----Task3-----\nUsers with authorization to update data");
        dataContext.UsersData.Join(dataContext.UserAccesses,
                ud => ud.Id, ua => ua.UserId,
                (ud, ua) => new { ud, ua }).Join(dataContext.UserRoles,
                udua => udua.ua.RoleId, ur => ur.Id,
                (udua, ur) => new { udua, ur }).Where(ur => ur.ur.CanUpdate == 1)
            .Select(ud => ud.udua.ud.Name)
            .ForEachAsync(Console.WriteLine).Wait();
        Console.WriteLine("-----Task4-----\nNumber of users who's phone starts with '555'");
        Console.WriteLine(dataContext.UsersData.Count(ud => ud.Phone.StartsWith("+555")));
        Console.WriteLine("-----Task5-----\nRoles and users");
        dataContext.UserRoles.GroupJoin(dataContext.UserAccesses,
                ur => ur.Id, ua => ua.RoleId,
                (ur, ua) => new { Role = ur, Users = string.Join(',', ua.
                            Join(dataContext.UsersData, ua => ua.UserId, ud => ud.Id,
                                (ua, ud) => ud.Name))})
            .ForEachAsync(pair => Console.WriteLine($"{pair.Role.Id} {pair.Users}")).Wait();
        //dataContext.UserRoles.GroupJoin(dataContext.UserAccesses,
        //    ur => ur.Id, ua => ua.RoleId,
        //    (ur, ua) => new { ur, ua }).GroupBy(urua => urua.ur.Id).ForEachAsync(Console.WriteLine).Wait();
        //dataContext.UserAccesses.GroupJoin(dataContext.UsersData,
        //        ua => ua.UserId, ud => ud.Id,
        //        (ua, ud) => new {ua, ud})
        //    .GroupBy(uaud => uaud.ua.RoleId)
        //    .ForEachAsync(Console.WriteLine).Wait();
        //var inner = dataContext.UserAccesses.GroupJoin(dataContext.UserRoles,
        //    ua => ua.RoleId, ur => ur.Id,
        //    (ua, ur) => new { ua, ur }); //$"{ur.Id} {string.Join(',', ua.Select(a => a.UserId))}")
        //dataContext.UserAccesses.GroupJoin(dataContext.UserRoles,
        //        ua => ua.RoleId, ur => ur.Id,
        //        (ua, ur) => new { ua, ur }).Join(dataContext.UsersData,
        //        uaur => uaur.ua.UserId, ud => ud.Id,
        //        (uaur, ud) => new{uaur, ud})
        //    .Select(arg => arg.uaur.ua.RoleId)
        //    .ForEachAsync(Console.WriteLine).Wait();
    }
}

/* EntityFramework
 * ORM фреймворк с расширенной функциональностью работы с БД
 *
 * Установка
 * - Ядро: Microsoft.EntityFrameworkCore
 * - Драйвер БД (в зависимости от СУБД): Microsoft.EntityFrameworkCore.SqlServer
 * - Инструментарий командной строки: Microsoft.EntityFrameworkCore.Tools
 *
 * Организация:
 * - DBContext - контекст данных, "образ" всей базы данных, объект, который содержит информацию о всех таблицах,
 * их связи и т.д.
 * - В зависимости от задач, работа с данными делится на%
 *  = Code First - когда БД еще нет и есть возможность начать с C# кода
 *  = Data First - когда БД уже существует и есть потребность создать ORM-объекты (Entities)
 * Далее считаем, что используется Code First
 * - Описываем ORM-объекты (Entities) и обозначаем их коллекции в DBContext. Связи между ними описываем в
 *  OnModelCreating().
 * - Задаем строку подключения к БД, на момент ее создания БД еще не существует. То есть строка подключения - к БД,
 * которая будет создана средствами EF. Передаем ее в контекст данных.
 * - Когда опись контекста выполнена создаем миграцию - промежуточный код для переноса контекста в настоящую БД
 * Используем консоль менеджера пакетов (PM Console) -> Tools -> Nuget ->
 * - Используем миграцию - переносим ее в БД
 *
 * - При необоходимости внесения изменений / дополнений
 * = вносим изменения в сущности / контексты (например, описываем дополнительную сущность, добавляем ее в DbSet)
 * = создаем миграцию Add-Migration Name
 * = применяем миграцию - update
 *
 *  EF: запросы, работа с данными
 *  Элементы DbSet, указанные в контексте, играют роль в коллекции, но есть отличия
 *  - для них действует разновидность LINQ-to-SQL, который возвращает тип IQeuryable
 *  - IQueryable отличается тем, что он отвечает за построение SQL-запроса, то есть по анализу LINQ-запроса
 *  будет создан запрос, потом выполнен, потом итерован
 *  - Iqueryable, если содержит ссылки на переменные, подставляет их на момент выполнения запроса. Если данные меняются
 * (например в цикле), то на запрос они уже не влияют
 *  - Поскольку они базируются на ADO(DataReader) в цикле-итераторе нельзя запускать другие запросы
 * - Не все выражения языка (C#) могут быть переведены на язык SQL, поэтому употребление ограниченное
*/