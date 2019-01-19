using System;
using Concepts;
using Concepts.SMS;
using Dolittle.Domain;
using Dolittle.Runtime.Events;
using Events.SMS;

namespace Domain.SMS
{
    public class TextMessagesFromUnknownSender : AggregateRoot
    {
        public TextMessagesFromUnknownSender(EventSourceId id) : base(id)
        {

        }

        public void ReceivedMessage(MessageId id, PhoneNumber sender, Message text, DateTimeOffset received)
        {
            Apply(new TextMessageReceivedFromUnknown(id, sender, text, received));
        }
    }
}
