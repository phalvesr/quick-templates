namespace MyTemplate.Entrypoint;

public record struct Result(bool IsSuccess, Exception? Error)
{
    public static Result Success() => new(true, null);
    public static Result Failure(Exception error) => new(false, error);

    public readonly bool IsFailure => !IsSuccess;

    public static implicit operator bool(Result result) => result.IsSuccess;
    public static implicit operator Result(Exception error) => Failure(error);
}