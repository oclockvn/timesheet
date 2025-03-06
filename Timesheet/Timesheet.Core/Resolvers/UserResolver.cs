using Timesheet.Core.Models;

namespace Timesheet.Core.Resolvers;

public interface IUserResolver
{
    Task<UserModel> Resolve();
}

// UNDONE: move to http context
internal class HttpContextUserResolver : IUserResolver
{
    public Task<UserModel> Resolve()
    {
        throw new NotImplementedException();
    }
}

public class AutomationUserResolver : IUserResolver
{
    public Task<UserModel> Resolve()
    {
        throw new NotImplementedException();
    }
}
