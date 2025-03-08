using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Core;
using Timesheet.Core.Models;
using Timesheet.Core.Resolvers;

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

        services.AddScoped<IUserResolver>(sp =>
        {
            var userResolver = A.Fake<IUserResolver>();
            A.CallTo(() => userResolver.Resolve()).Returns(Task.FromResult(new UserModel { Id = 1 }));
            return userResolver;
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