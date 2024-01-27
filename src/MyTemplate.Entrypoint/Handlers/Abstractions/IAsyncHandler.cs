namespace MyTemplate.Entrypoint.Handlers.Abstractions;

public interface IAsyncHandler<THandle>
{
    Task<Result> HandleAsync(THandle input, CancellationToken cancellationToken);
}
