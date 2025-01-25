using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DataManagement.EF.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Model { get; set; } = null!;
    public string Producer { get; set; } = null!;
    public double Price { get; set; }
}