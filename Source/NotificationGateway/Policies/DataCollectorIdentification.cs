using Dolittle.Domain;
using Dolittle.Events.Processing;
using Dolittle.ReadModels;
using Dolittle.Runtime.Commands;
using Dolittle.Runtime.Commands.Coordination;
using Dolittle.Runtime.Events;
using Domain.SMS;
using Events.SMS;
using Read.DataCollector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Policies
{
    public class DataCollectorIdentification : ICanProcessEvents
    {
        private readonly IReadModelRepositoryFor<DataCollector> _dataCollectors;
        private readonly ICommandContextManager _commandContextManager;
        private readonly IAggregateRootRepositoryFor<TextMessagesFromDataCollector> _dataCollectorMessages;
        private readonly IAggregateRootRepositoryFor<TextMessagesFromUnknownSender> _unknownSenderMessages;

        public DataCollectorIdentification(
            IReadModelRepositoryFor<DataCollector> dataCollectors,
            ICommandContextManager commandContextManager,
            IAggregateRootRepositoryFor<TextMessagesFromDataCollector> dataCollectorMessages,
            IAggregateRootRepositoryFor<TextMessagesFromUnknownSender> unknownSenderMessages)
        {
            _dataCollectors = dataCollectors;
            _commandContextManager = commandContextManager;
            _dataCollectorMessages = dataCollectorMessages;
            _unknownSenderMessages = unknownSenderMessages;
        }

        [EventProcessor("2C718449-6483-4F70-91D7-12FE55094809")]
        public void Process(TextMessageReceived @event)
        {
            var commandContext = _commandContextManager.EstablishForCommand(new CommandRequest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                1,
                new Dictionary<string, object>()));

            var dataCollector = _dataCollectors.Query
                .Where(dc => dc.PhoneNumber == @event.Sender)
                .SingleOrDefault();

            if (dataCollector != null)
            {
                var dataCollectorMessages = _dataCollectorMessages.Get((Guid)dataCollector.Id);
                dataCollectorMessages.ReceivedMessage(
                    @event.Id,
                    @event.Text,
                    @event.Received);
            }
            else
            {
                var unknownSenderMessages = _unknownSenderMessages.Get(EventSourceId.Empty);
                unknownSenderMessages.ReceivedMessage(
                    @event.Id,
                    @event.Sender,
                    @event.Text,
                    @event.Received);
            }
            commandContext.Commit();
        }
    }
}
