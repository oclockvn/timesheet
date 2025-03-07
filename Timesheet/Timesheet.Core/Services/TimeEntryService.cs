using Timesheet.Core.Db;
using Timesheet.Core.Db.Entity;
using Timesheet.Core.Models;
using Timesheet.Core.Resolvers;

namespace Timesheet.Core.Services;

public interface ITimeEntryService
{
    //Task<TimeEntryDetailModel?> GetActiveTimeEntryAsync();
    Task<Result<TimeEntryDetailModel>> StartTrackingAsync(CreateTimeEntryModel model, CancellationToken cancellationToken = default);
}

internal class TimeEntryService(IUserResolver userResolver, IEntryContextService entryContextService, ApplicationDbContext dbContext) : ITimeEntryService
{
    public async Task<Result<TimeEntryDetailModel>> StartTrackingAsync(CreateTimeEntryModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(model.Description);

        var context = await entryContextService.GetEntryContextAsync(model.Description, cancellationToken);
        if (!context.IsSuccess)
            return Result<TimeEntryDetailModel>.Failure(context.Code);

        var currentUser = await userResolver.Resolve();

        TimeEntry timeEntry = new()
        {
            StartTimeUtc = DateTime.UtcNow,
            EndTimeUtc = null,
            Description = model.Description,
            UserId = currentUser.Id,
            TaskId = context.Value.TaskId,
        };

        dbContext.TimeEntries.Add(timeEntry);
        await dbContext.SaveChangesAsync(cancellationToken);

        TimeEntryDetailModel result = new()
        {
            Id = timeEntry.Id,
            StartTimeUtc = timeEntry.StartTimeUtc,
            EndTimeUtc = timeEntry.EndTimeUtc,
            Description = timeEntry.Description,
            UserId = timeEntry.UserId,
            TaskId = timeEntry.TaskId,
        };

        return Result<TimeEntryDetailModel>.Success(result);
    }

    //public async Task<TimeEntryDetailModel?> GetActiveTimeEntryAsync()
    //{
    //    var currentUser = await userResolver.Resolve();
    //    return await dbContext.TimeEntries
    //        .FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.EndTime == null);
    //}
}