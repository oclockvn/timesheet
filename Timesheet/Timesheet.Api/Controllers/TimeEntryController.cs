using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Models;
using Timesheet.Core.Services;

namespace Timesheet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEntryController : ControllerBase
{
    private readonly ITimeEntryService _timeEntryService;

    public TimeEntryController(ITimeEntryService timeEntryService)
    {
        _timeEntryService = timeEntryService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<TimeEntryDetailModel>> StartTracking([FromBody] CreateTimeEntryModel request)
    {
        var timeEntry = await _timeEntryService.StartTrackingAsync(request);
        return Ok(timeEntry);
    }

    [HttpPost("{id}/stop")]
    public async Task<ActionResult<TimeEntryDetailModel>> StopTracking(int id)
    {
        var timeEntry = await _timeEntryService.StopTrackingAsync(id);
        return Ok(timeEntry);
    }

    [HttpGet("active")]
    public async Task<ActionResult<TimeEntryDetailModel?>> GetActiveTimeEntry()
    {
        var timeEntry = await _timeEntryService.GetActiveTimeEntryAsync();
        return Ok(timeEntry);
    }
}

public record StartTrackingRequest(string TaskCode, string? ProjectCode = null, string? Description = null); 