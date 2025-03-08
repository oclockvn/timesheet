using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Timesheet.Core.Db;
using Timesheet.Core.Resolvers;
using Timesheet.Core.Services;

namespace Timesheet.Core;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTimesheetCliCore(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false, Action<DbContextOptionsBuilder>? dbBuilder = null)
    {
        services
            .AddApplicationDbContext(configuration, isDevelopment, dbBuilder)
            .AddScoped<IUserResolver, HttpContextUserResolver>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<ITimeEntryService, TimeEntryService>()
            .AddScoped<IEntryContextService, EntryContextService>()
            ;

        return services;
    }

    private static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false, Action<DbContextOptionsBuilder>? dbBuilder = null)
    {
        services.Configure<DbOption>(x => x.ConnectionString = configuration.GetConnectionString("DefaultConnection")!);
        services.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
        {
            options
                .EnableSensitiveDataLogging(isDevelopment)
                .EnableDetailedErrors(true);

            if (dbBuilder is not null)
            {
                dbBuilder(options);
            }
            else
            {
                var connectionString = sp.GetRequiredService<IOptions<DbOption>>().Value.ConnectionString;
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("Connection string is required");
                }

                options.UseSqlServer(connectionString);
            }

        }, ServiceLifetime.Scoped);

        return services;
    }
}
