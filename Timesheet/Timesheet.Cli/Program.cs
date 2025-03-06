// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => { })
    //.ConfigureServices((context, services) => services.AddTimesheetCliCore(context.Configuration, context.HostingEnvironment.IsDevelopment()))
    .Build();

await builder.RunAsync();
