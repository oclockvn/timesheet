using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Timesheet.Core.Db;
using Timesheet.Core.Resolvers;

namespace Timesheet.Core.Services;

public record struct EntryContext(int TaskId, string TaskCode, int? ProjectId);

public interface IEntryContextService
{
    Task<Result<EntryContext>> GetEntryContextAsync(string description, CancellationToken cancellationToken = default);
}

public partial class EntryContextService(ApplicationDbContext db, IUserResolver userResolver) : IEntryContextService
{
    public async Task<Result<EntryContext>> GetEntryContextAsync(string description, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        // Extract task code and description based on different input formats
        string? taskCode = null;
        string cleanDescription = description;

        // Check for format: [PROJ-1234] working on user service
        var match = SquareBracketsPattern().Match(description);
        if (match.Success)
        {
            taskCode = match.Groups[1].Value;
            cleanDescription = match.Groups[2].Value.Trim();
        }
        // Check for format: PROJ-1234: working on user service
        else if (description.Contains(':'))
        {
            var parts = description.Split(":", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            taskCode = parts[0].Trim();
            cleanDescription = parts[1].Trim();
        }

        if (string.IsNullOrWhiteSpace(taskCode))
        {
            return Result<EntryContext>.Failure(ResultValues.TaskCodeNotFound);
        }

        var task = await db.Tasks.Where(x => x.Code == taskCode)
            .Select(x => new Db.Entity.Task2 { Id = x.Id, ProjectId = x.ProjectId })
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);

        var user = await userResolver.Resolve();
        if (task is null)
        {
            // add new task
            task = new Db.Entity.Task2
            {
                Code = taskCode,
                Description = cleanDescription,
                UserId = user.Id,
                //ProjectId = 1 // UNDONE: add project to task
            };
            db.Tasks.Add(task);
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result<EntryContext>.Success(new EntryContext(task.Id, taskCode, null));
    }

    [GeneratedRegex(@"^\[([^\]]+)\]\s*(.+)$", RegexOptions.Compiled)]
    private static partial Regex SquareBracketsPattern();
}
