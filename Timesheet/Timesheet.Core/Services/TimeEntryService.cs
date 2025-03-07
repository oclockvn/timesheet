using Microsoft.EntityFrameworkCore;
using Timesheet.Core.Db;
using Timesheet.Core.Db.Entity;
using Timesheet.Core.Models;
using Timesheet.Core.Resolvers;

namespace Timesheet.Core.Services;

public interface ITimeEntryService
{
    Task<TimeEntryDetailModel> StartTrackingAsync(CreateTimeEntryModel model);
    //Task<TimeEntryDetailModel> StopTrackingAsync(int timeEntryId);
    Task<TimeEntryDetailModel?> GetActiveTimeEntryAsync();
}

internal class TimeEntryService(IUserResolver userResolver, ApplicationDbContext dbContext) : ITimeEntryService
{
    public async Task<TimeEntryDetailModel> StartTrackingAsync(CreateTimeEntryModel model)
    {
        var currentUser = await userResolver.Resolve();
        TimeEntry timeEntry = new()
        {
            StartTimeUtc = DateTime.UtcNow,
            Description = model.Description,
            UserId = currentUser.Id,
            //TaskId = 0,
        };

        dbContext.TimeEntries.Add(timeEntry);
        await dbContext.SaveChangesAsync();

        return new TimeEntryDetailModel
        {
            Id = timeEntry.Id,
            StartTimeUtc = timeEntry.StartTimeUtc,
            EndTimeUtc = timeEntry.EndTimeUtc,
            Description = timeEntry.Description,
            UserId = timeEntry.UserId,
            TaskId = timeEntry.TaskId,
            //Duration = timeEntry.Duration
        };
    }

    //public async Task<TimeEntryDetailModel> StopTrackingAsync(int timeEntryId)
    //{
    //    var timeEntry = await dbContext.TimeEntries.FindAsync(timeEntryId);
    //    if (timeEntry == null)
    //        throw new ArgumentException("Time entry not found");

    //    timeEntry.EndTime = DateTime.UtcNow;
    //    await dbContext.SaveChangesAsync();

    //    return timeEntry;
    //}

    public async Task<TimeEntryDetailModel?> GetActiveTimeEntryAsync()
    {
        var currentUser = await userResolver.Resolve();
        return await dbContext.TimeEntries
            .FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.EndTime == null);
    }
}