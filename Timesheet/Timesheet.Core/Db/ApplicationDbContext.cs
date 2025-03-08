using Microsoft.EntityFrameworkCore;
using Timesheet.Core.Db.Entity;
using Timesheet.Core.Resolvers;

namespace Timesheet.Core.Db;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUserResolver userResolver) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Task2> Tasks { get; set; } = null!;
    public DbSet<TimeEntry> TimeEntries { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await ApplyAuditAsync();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async System.Threading.Tasks.Task ApplyAuditAsync()
    {
        var user = await userResolver.Resolve();

        // Create a stable snapshot of entries and filter by state
        var addedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added)
            .ToList();

        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        // Process added entries
        foreach (var entry in addedEntries)
        {
            if (entry.Entity is ICreatedBy createEntity)
            {
                createEntity.CreatedUtc = DateTime.UtcNow;
                createEntity.CreatedBy = user.Id;
                createEntity.CreatedName = user.FullName;
            }
        }

        // Process modified entries
        foreach (var entry in modifiedEntries)
        {
            if (entry.Entity is IUpdatedBy updateEntity)
            {
                updateEntity.UpdatedUtc = DateTime.UtcNow;
                updateEntity.UpdatedBy = user.Id;
                updateEntity.UpdatedName = user.FullName;
            }
        }
    }

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
        modelBuilder.Entity<Task2>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();

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
