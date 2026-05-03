using System;
using System.IO;
using ImperiumAO.Server;
using ImperiumAO.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.Tasks;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Starting ImperiumAO Server");

    var services = new ServiceCollection();

    services.AddLogging(loggingBuilder =>
        loggingBuilder.AddSerilog());

    services.AddConfiguration(configuration);
    services.AddGameServices();

    var serviceProvider = services.BuildServiceProvider();

    // Apply database migrations
    try
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ImperiumAOContext>();
        await dbContext.Database.MigrateAsync();
        Log.Information("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error applying database migrations");
    }

    var gameServer = serviceProvider.GetRequiredService<GameServer>();

    await gameServer.StartAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

