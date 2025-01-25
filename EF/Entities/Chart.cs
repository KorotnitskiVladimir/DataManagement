namespace DataManagement.EF.Entities;

public class Chart
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Products { get; set; } = null!;
    public double Value { get; set; }
}