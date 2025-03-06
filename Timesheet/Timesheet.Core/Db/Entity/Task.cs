using System.ComponentModel.DataAnnotations;

namespace Timesheet.Core.Db.Entity;

public class Task : ICreatedBy, IUpdatedBy
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Code { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    // Navigation properties
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int? ProjectId { get; set; }  // Optional, a task can exist without a project
    public Project? Project { get; set; }

    public ICollection<TimeEntry> TimeEntries { get; set; } = [];

    public DateTime? UpdatedUtc { get; set; }
    public int? UpdatedBy { get; set; }
    [StringLength(100)]
    public string? UpdatedName { get; set; }

    public DateTime CreatedUtc { get; set; }
    public int CreatedBy { get; set; }
    [StringLength(100)]
    public string CreatedName { get; set; } = null!;
}