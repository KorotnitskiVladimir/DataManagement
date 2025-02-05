namespace DataManagement.EF.Entities;

public class UserAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RoleId { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public string Dk { get; set; } = null!;

    public override string ToString()
    {
        return $"UserAccess: Id({Id}), UserId({UserId}), RoleId({RoleId}), Login({Login})";
    }
}