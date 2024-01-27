using Amazon.Lambda.SQSEvents;

namespace MyTemplate.CrossCutting.CorrelationIds;

public interface IScoppedCorrelationId
{
    Guid CorrelationId { get; }
}

internal interface IScoppedCorrelationIdProvider : IScoppedCorrelationId 
{
    void TryLoadFromMessageAttributes(string key, IDictionary<string, SQSEvent.MessageAttribute> messageAttributes);
}
