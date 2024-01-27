
using Amazon.Lambda.SQSEvents;

namespace MyTemplate.CrossCutting.CorrelationIds;

public class ScoppedCorrelationIdProvider : IScoppedCorrelationIdProvider
{
    private Guid _correlationId = Guid.Empty;

    public Guid CorrelationId => _correlationId;

    public void TryLoadFromMessageAttributes(string key, IDictionary<string, SQSEvent.MessageAttribute> messageAttributes)
    {
        if (messageAttributes.TryGetValue(key, out var messageAttribute) 
            && !string.IsNullOrWhiteSpace(messageAttribute.StringValue)
            && Guid.TryParse(messageAttribute.StringValue, out var correlationId))
        {
            _correlationId = correlationId;
        }
    }
}
