using Microsoft.EntityFrameworkCore;
using TimesheetCli.Core.Db.Entity;

namespace TimesheetCli.Core.Db;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Entity.Task> Tasks { get; set; } = null!;
    public DbSet<TimeEntry> TimeEntries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(e => e.Id);

            // Configure relationship with User
            entity.HasOne(p => p.User)
                  .WithMany(u => u.Projects)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Task entity
        modelBuilder.Entity<Entity.Task>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(e => e.Id);

            // Configure relationship with User
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Project (optional)
            entity.HasOne(t => t.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(t => t.ProjectId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);
        });

        // Configure TimeEntry entity
        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.ToTable("TimeEntries");
            entity.HasKey(e => e.Id);

            // Configure relationship with User
            entity.HasOne(te => te.User)
                  .WithMany(u => u.TimeEntries)
                  .HasForeignKey(te => te.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Task
            entity.HasOne(te => te.Task)
                  .WithMany(t => t.TimeEntries)
                  .HasForeignKey(te => te.TaskId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Ignore computed property
            entity.Ignore(te => te.Duration);
        });
    }
}
