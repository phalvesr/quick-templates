using System.Text.Json;
using Amazon.Lambda.SQSEvents;
using MyTemplate.CrossCutting.CorrelationIds;
using MyTemplate.Entrypoint.Handlers.Abstractions;
using Serilog;
using Serilog.Context;

namespace MyTemplate.Entrypoint.Decorators;

internal class SqsBatchResponseDecorator
{
    private readonly ILogger _logger;
    private readonly IScoppedCorrelationIdProvider _correlationIdProvider;

    public SqsBatchResponseDecorator(
        ILogger logger, 
        IScoppedCorrelationIdProvider correlationIdProvider)
    {
        _logger = logger;
        _correlationIdProvider = correlationIdProvider;
    }

    internal async Task<SQSBatchResponse> DecorateAsync<THandle>(IAsyncHandler<THandle> asyncHandler, SQSEvent sqsEvent, CancellationToken cancellationToken)
    {
        var failedItems = new List<SQSBatchResponse.BatchItemFailure>();

        foreach (var record in sqsEvent.Records)
        {
            await TryProcessMessageWithAsyncHandlerAsync(asyncHandler, failedItems, record, cancellationToken);
        }

        return new SQSBatchResponse(failedItems);
    }

    private async Task TryProcessMessageWithAsyncHandlerAsync<THandle>(
        IAsyncHandler<THandle> asyncHandler, 
        List<SQSBatchResponse.BatchItemFailure> failedItems, 
        SQSEvent.SQSMessage record, 
        CancellationToken cancellationToken)
    {
        _correlationIdProvider.TryLoadFromMessageAttributes("x-correlation-id", record!.MessageAttributes);
        
        using (LogContext.PushProperty("CorrelationId", _correlationIdProvider.CorrelationId))
        {
            try
            {
                var input = JsonSerializer.Deserialize<THandle>(record.Body);

                var result = await asyncHandler.HandleAsync(input!, cancellationToken);
            
                if (result.IsFailure)
                {
                    failedItems.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = record.MessageId });
                    _logger.Error(result.Error, "Error processing message. Error message {ErrorMessage}", result.Error!.Message);
                } 
                else 
                {
                    _logger.Information("Message processed successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing message");
                failedItems.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = record.MessageId });
            }
        }
    }

    internal SQSBatchResponse Decorate<THandle>(IHandler<THandle> handler, SQSEvent sqsEvent)
    {
        var failedItems = new List<SQSBatchResponse.BatchItemFailure>();

        foreach (var record in sqsEvent.Records)
        {
            TryProcessMessageWithHandler(handler, failedItems, record);
        }

        return new SQSBatchResponse(failedItems);   
    }

    private void TryProcessMessageWithHandler<THandle>(IHandler<THandle> handler, List<SQSBatchResponse.BatchItemFailure> failedItems, SQSEvent.SQSMessage record)
    {
        _correlationIdProvider.TryLoadFromMessageAttributes("x-correlation-id", record!.MessageAttributes);
        
        using (LogContext.PushProperty("CorrelationId", _correlationIdProvider.CorrelationId))
        {
            try
            {
                var input = JsonSerializer.Deserialize<THandle>(record.Body);

                var result = handler.Handle(input!);
            
                if (result.IsFailure)
                {
                    failedItems.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = record.MessageId });
                    _logger.Error(result.Error, "Error processing message. Error message {ErrorMessage}", result.Error!.Message);
                } 
                else 
                {
                    _logger.Information("Message processed successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing message");
                failedItems.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = record.MessageId });
            }
        }
    }
}
