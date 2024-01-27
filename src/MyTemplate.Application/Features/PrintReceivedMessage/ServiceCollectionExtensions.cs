using Microsoft.Extensions.DependencyInjection;

namespace MyTemplate.Application.Features.PrintReceivedMessage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPrintReceivedMessageFeature(this IServiceCollection services)
    {
        services.AddScoped<IPrintReceivedMessageUseCase, PrintReceivedMessageUseCase>();
        
        return services;
    }
}
