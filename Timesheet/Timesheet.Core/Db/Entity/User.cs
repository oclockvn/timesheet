using System.ComponentModel.DataAnnotations;

namespace Timesheet.Core.Db.Entity
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(250)]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; }

        public ICollection<Project> Projects { get; set; } = [];
        public ICollection<Task2> Tasks { get; set; } = [];
        public ICollection<TimeEntry> TimeEntries { get; set; } = [];
    }
}