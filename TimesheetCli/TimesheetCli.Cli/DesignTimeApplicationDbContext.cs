using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TimesheetCli.Core.Db;

namespace TimesheetCli.Cli;

public class DesignTimeApplicationDbContext : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<ApplicationDbContext>()
            .Build();

        var connectionString = GetConnectionStringFromArgs(args);
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Connection string is not provided");

        var dbOption = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ApplicationDbContext(dbOption);
    }

    private static string GetConnectionStringFromArgs(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--connection" && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }

        return string.Empty;
    }
}
