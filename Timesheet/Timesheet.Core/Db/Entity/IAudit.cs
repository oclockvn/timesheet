namespace TimesheetCli.Core.Db.Entity;

public interface ICreatedBy
{
    DateTime CreatedUtc { get; set; }
    int CreatedBy { get; set; }
    string CreatedName { get; set; }
}

public interface IUpdatedBy
{
    DateTime? UpdatedUtc { get; set; }
    int? UpdatedBy { get; set; }
    string? UpdatedName { get; set; }
}
