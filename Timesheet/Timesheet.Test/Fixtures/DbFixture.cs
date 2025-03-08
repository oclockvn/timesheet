using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Core;

namespace Timesheet.Test.Fixtures;

public class DbFixture : IDisposable
{
    private readonly IServiceProvider _serviceProvider;

    public DbFixture()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .Build();

        services.AddTimesheetCliCore(configuration, isDevelopment: true, db =>
        {
            db.UseInMemoryDatabase("TestDb");
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    public T Get<T>() where T : class
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public void Dispose()
    {
    }
}