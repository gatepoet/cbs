using System;
using Concepts;
using Concepts.SMS;
using Dolittle.Domain;
using Dolittle.Runtime.Events;
using Events.SMS;

namespace Domain.SMS
{
    public class TextMessagesFromDataCollector : AggregateRoot
    {
        public TextMessagesFromDataCollector(EventSourceId id) : base(id)
        {
        }

        public void ReceivedMessage(MessageId id, Message text, DateTimeOffset received)
        {
            Apply(new TextMessageReceivedFromDataCollector(id, EventSourceId, text, received));
        }
    }
}
