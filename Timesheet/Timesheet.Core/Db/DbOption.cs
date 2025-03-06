namespace Timesheet.Core.Db;

public class DbOption
{
    public const string SECTION_NAME = "DbOption";
    public string ConnectionString { get; set; } = null!;
}
