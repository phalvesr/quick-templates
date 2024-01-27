using Microsoft.Extensions.DependencyInjection;
using MyTemplate.Infrastructure.Extensions;
using MyTemplate.Application.Extensions;
using MyTemplate.CrossCutting.Extensions;
using MyTemplate.Entrypoint.Decorators;
using MyTemplate.Entrypoint.Dtos;
using MyTemplate.Entrypoint.Handlers;
using MyTemplate.Entrypoint.Handlers.Abstractions;
using Microsoft.Extensions.Configuration;
using MyTemplate.CrossCutting.Helpers;
using Amazon.XRay.Recorder.Handlers.AwsSdk;

namespace MyTemplate.Entrypoint;

public class DependencyInjection
{
    public IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        ConfigureServices(services);

        OnBuildingServiceProvider(services);

        return services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        AddObservability(services);

        services
            .AddScoped<SqsBatchResponseDecorator>()
            .AddScoped<IAsyncHandler<SqsMessage>, PrintHandler>()
            .AddApplication()
            .AddInfrastructure()
            .AddLogger()
            .AddScoppedCorrelationIds();
    }

    private static IServiceCollection AddObservability(IServiceCollection services)
    {
        if (!EnvironmentHelpers.IsObservableEnvironment)
        {
            return services;
        }

        // To make your lambda function observable, you still need 
        // to add the xray permissions to your lambda execution role.
        // For more info check: https://docs.aws.amazon.com/aws-managed-policy/latest/reference/AWSXRayDaemonWriteAccess.html
        AWSSDKHandler.RegisterXRayForAllServices();

        return services;
    }

    /// <summary>
    /// Use this method to swap your services for mocks in your integration tests
    /// </summary>
    /// <param name="services"></param>
    protected void OnBuildingServiceProvider(IServiceCollection services) {  }
}
