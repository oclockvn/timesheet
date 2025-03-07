namespace Timesheet.Core.Models;

public record CreateTimeEntryModel(string TaskCode, string? ProjectCode = null, string? Description = null);

public record UpdateTimeEntryModel(string? Description = null);

public class TimeEntryDetailModel
{
    public int Id { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public string? Description { get; set; }
    public int UserId { get; set; }
    //public User User { get; set; } = null!;

    public int TaskId { get; set; }
    //public Task Task { get; set; } = null!;

    // Computed property
    public TimeSpan? Duration => EndTimeUtc.HasValue ? EndTimeUtc.Value - StartTimeUtc : null;

    //public DateTime CreatedUtc { get; set; }
    //public int CreatedBy { get; set; }
    //[Required, StringLength(100)]
    //public string CreatedName { get; set; } = null!;
}