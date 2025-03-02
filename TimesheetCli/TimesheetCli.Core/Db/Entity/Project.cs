using System.ComponentModel.DataAnnotations;

namespace TimesheetCli.Core.Db.Entity;

public class Project : ICreatedBy
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Foreign keys
    public int UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Task> Tasks { get; set; } = [];

    public DateTime CreatedUtc { get; set; }
    public int CreatedBy { get; set; }
    [Required, StringLength(100)]
    public string CreatedName { get; set; } = null!;
}
