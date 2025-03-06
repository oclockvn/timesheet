namespace Timesheet.Core.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = null!;
}
