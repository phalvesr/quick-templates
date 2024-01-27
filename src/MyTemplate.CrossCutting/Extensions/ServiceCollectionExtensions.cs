using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyTemplate.CrossCutting.CorrelationIds;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace MyTemplate.CrossCutting.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScoppedCorrelationIds(this IServiceCollection services)
    {
        services.TryAddScoped<ScoppedCorrelationIdProvider>();

        services.TryAddScoped<IScoppedCorrelationId>(
            sp => sp.GetRequiredService<ScoppedCorrelationIdProvider>()
        );
        services.TryAddScoped<IScoppedCorrelationIdProvider>(
            sp => sp.GetRequiredService<ScoppedCorrelationIdProvider>()
        );
        
        return services;
    }

    public static IServiceCollection AddLogger(this IServiceCollection services, LogEventLevel logLevel = LogEventLevel.Information)
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(new JsonFormatter(renderMessage: true))
            .MinimumLevel.Is(logLevel)
            .CreateLogger();

        services.TryAddSingleton<ILogger>(logger);

        return services;
    }
}
