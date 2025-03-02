using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TimesheetCli.Core.Db;
using TimesheetCli.Core.Resolvers;

namespace TimesheetCli.Core;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTimesheetCliCore(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
    {
        services
            .AddApplicationDbContext(configuration, isDevelopment)
            .AddScoped<IUserResolver, HttpContextUserResolver>()
            ;

        return services;
    }

    private static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
    {
        services.Configure<DbOption>(x => x.ConnectionString = configuration.GetConnectionString("DefaultConnection")!);
        services.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
        {
            var connectionString = sp.GetRequiredService<IOptions<DbOption>>().Value.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string is required");
            }

            options.UseSqlServer(connectionString)
            .EnableSensitiveDataLogging(isDevelopment)
            .EnableDetailedErrors(true)
            ;
        }, ServiceLifetime.Scoped);

        return services;
    }
}
