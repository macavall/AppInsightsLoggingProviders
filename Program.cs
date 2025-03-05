using Google.Protobuf.WellKnownTypes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class Program
{
    public static IHost app;

    public static void Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.ConfigureFunctionsWebApplication();

        // Register the category collector as a singleton
        builder.Services.AddSingleton<ICategoryCollector, CategoryCollector>();

        // Configure logging
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole(); // Output logs to the console
            logging.SetMinimumLevel(LogLevel.Trace); // Capture all log levels
            logging.AddFilter(null, LogLevel.Trace); // Allow all categories at Trace level
        });

        // Add the custom logger provider
        builder.Services.AddSingleton<ILoggerProvider, CategoryCollectingLoggerProvider>();

        // Application Insights configuration
        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights();

        app = builder.Build();

        NewMethod();

        app.Run();
    }

    public static void NewMethod()
    {
        // Demonstrate the collector (optional)
        var collector = app.Services.GetRequiredService<ICategoryCollector>();
        var categories = collector.GetCategories();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Categories collected so far: {Categories}", string.Join(", ", categories));
    }
}

// Define the interface and classes inline for completeness
public interface ICategoryCollector
{
    void AddCategory(string category);
    IEnumerable<string> GetCategories();
}

public class CategoryCollector : ICategoryCollector
{
    private readonly ConcurrentDictionary<string, byte> _categories = new ConcurrentDictionary<string, byte>();

    public void AddCategory(string category)
    {
        _categories.TryAdd(category, 0);
    }

    public IEnumerable<string> GetCategories()
    {
        return _categories.Keys;
    }
}

public class CategoryCollectingLoggerProvider : ILoggerProvider
{
    private readonly ICategoryCollector _collector;

    public CategoryCollectingLoggerProvider(ICategoryCollector collector)
    {
        _collector = collector;
    }

    public ILogger CreateLogger(string categoryName)
    {
        _collector.AddCategory(categoryName);
        return NullLogger.Instance;
    }

    public void Dispose() { }
}