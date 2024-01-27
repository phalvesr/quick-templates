namespace MyTemplate.Entrypoint.Handlers.Abstractions;

public interface IHandler<THandle>
{
    Result Handle(THandle input);
}
