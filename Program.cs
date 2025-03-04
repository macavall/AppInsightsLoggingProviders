using Google.Protobuf.WellKnownTypes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();

        // Configure logging to show all categories
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole(); // Output logs to the console
            logging.SetMinimumLevel(LogLevel.Trace); // Capture all log levels
            logging.AddFilter(null, LogLevel.Trace); // Allow all categories at Trace level
        });

        // Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        var app = builder.Build();

        // Log a message to indicate logging setup
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Logging is configured to show all available categories. Check the console output for log messages and their categories.");


        app.Run();
    }
}