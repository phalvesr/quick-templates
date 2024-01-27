using Serilog;

namespace MyTemplate.Application.Features.PrintReceivedMessage;

public class PrintReceivedMessageUseCase : IPrintReceivedMessageUseCase
{
    private readonly ILogger _logger;

    public PrintReceivedMessageUseCase(ILogger logger)
    {
        _logger = logger;
    }

    public void Execute(string message)
    {
        _logger.Information("Received {message}", message);
    }
}
