using System.Text.Json.Serialization;

namespace MyTemplate.Entrypoint.Dtos;

public class SqsMessage
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}
