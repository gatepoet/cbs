using System;
using Dolittle.Concepts;
using Dolittle.Events;

namespace Events.SMS
{
    public class TextMessageReceivedFromDataCollector : Value<TextMessageReceivedFromDataCollector>, IEvent
    {
        public TextMessageReceivedFromDataCollector(Guid id, Guid dataCollectorId, string text, DateTimeOffset received)
        {
            Id = id;
            Text = text;
            DataCollectorId = dataCollectorId;
            Received = received;
        }

        public Guid Id { get; }
        public Guid DataCollectorId { get; }
        public string Text { get; }
        public DateTimeOffset Received { get; }
    }
}
