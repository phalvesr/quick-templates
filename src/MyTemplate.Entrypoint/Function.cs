using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using MyTemplate.CrossCutting.Helpers;
using MyTemplate.Entrypoint.Decorators;
using MyTemplate.Entrypoint.Dtos;
using MyTemplate.Entrypoint.Handlers.Abstractions;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyTemplate.Entrypoint;

public class Function
{
    private IServiceProvider _serviceProvider;

    // Integration tests can use this constructor to inject dependencies
    internal Function(DependencyInjection dependencyInjection)
    {
        _serviceProvider = dependencyInjection.BuildServiceProvider();
    }
    
    // Lambda uses this constructor
    public Function()
    {
        _serviceProvider = new DependencyInjection().BuildServiceProvider();
    }

    public async Task<SQSBatchResponse> FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        var cancellationToken = GetCancellationTokenFromLambdaContext(context);

        using var scope = _serviceProvider.CreateScope();
        
        var handler = scope.ServiceProvider.GetRequiredService<IAsyncHandler<SqsMessage>>();

        var decorator = scope.ServiceProvider.GetRequiredService<SqsBatchResponseDecorator>();

        return await decorator.DecorateAsync(handler, sqsEvent, cancellationToken);
    }

    private static CancellationToken GetCancellationTokenFromLambdaContext(ILambdaContext context)
    {
        if (EnvironmentHelpers.IsLocalEnvironment)
        {
            return CancellationToken.None;
        }

        var cts = new CancellationTokenSource();

        cts.CancelAfter(context.RemainingTime.Subtract(TimeSpan.FromSeconds(1)));

        return cts.Token;
    }
}
