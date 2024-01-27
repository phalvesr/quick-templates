using MyTemplate.Application.Features.PrintReceivedMessage;
using MyTemplate.Entrypoint.Dtos;
using MyTemplate.Entrypoint.Handlers.Abstractions;
using Serilog;

namespace MyTemplate.Entrypoint.Handlers;

public class PrintHandler : IAsyncHandler<SqsMessage>
{
    private ILogger _logger;
    private readonly IPrintReceivedMessageUseCase _pringReceivedMessage;

    public PrintHandler(
        ILogger logger, 
        IPrintReceivedMessageUseCase pringReceivedMessage)
    {
        _logger = logger;
        _pringReceivedMessage = pringReceivedMessage;
    }

    public Task<Result> HandleAsync(SqsMessage input, CancellationToken cancellationToken)
    {
        _pringReceivedMessage.Execute(input.Message);

        return Task.FromResult(Result.Success());
    }
}
