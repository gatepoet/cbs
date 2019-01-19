using System;
using Dolittle.Concepts;
using Dolittle.Events;

namespace Events.SMS
{
    public class TextMessageReceivedFromUnknown : Value<TextMessageReceivedFromUnknown>, IEvent
    {
        public TextMessageReceivedFromUnknown(Guid id, string sender, string text, DateTimeOffset received)
        {
            Id = id;
            Sender = sender;
            Text = text;
            Received = received;
        }

        public Guid Id { get; }
        public string Sender { get; }
        public string Text { get; }
        public DateTimeOffset Received { get; }
    }
}
