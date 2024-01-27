using Microsoft.Extensions.DependencyInjection;
using MyTemplate.Application.Features.PrintReceivedMessage;

namespace MyTemplate.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddPrintReceivedMessageFeature();
        
        return services;
    }
}
