using System.ComponentModel.DataAnnotations;

namespace Timesheet.Core.Db.Entity
{
    public class TimeEntry : ICreatedBy
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int TaskId { get; set; }
        public Task Task { get; set; } = null!;

        // Computed property
        public TimeSpan? Duration => EndTimeUtc.HasValue ? EndTimeUtc.Value - StartTimeUtc : null;

        public DateTime CreatedUtc { get; set; }
        public int CreatedBy { get; set; }
        [Required, StringLength(100)]
        public string CreatedName { get; set; } = null!;
    }
}