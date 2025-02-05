namespace DataManagement.EF.Entities;

public class UserData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public override string ToString()
    {
        return $"UserData: Id ({Id}, Name ({Name}), Email: ({Email}), Phone({Phone})";
    }
}