using Dolittle.Domain;
using Dolittle.Events;
using Dolittle.Events.Coordination;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.ReadModels;
using Dolittle.Runtime.Commands;
using Dolittle.Runtime.Commands.Coordination;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Coordination;
using Dolittle.Security;
using Domain.SMS;
using Events.SMS;
using Machine.Specifications;
using Moq;
using Read.DataCollector;
using System;
using System.Globalization;
using System.Linq;
using It = Machine.Specifications.It;

namespace Policies.Specs
{
    [Subject(typeof(DataCollectorIdentification))]
    public class Receiving_message_from_data_collector
    {
        static TextMessageReceived _event;
        static DataCollectorIdentification _lookup;
        static UncommittedEvents _producedEvents;

        static Mock<IReadModelRepositoryFor<DataCollector>> _dataCollectors;
        static Mock<IAggregateRootRepositoryFor<TextMessagesFromDataCollector>> _dataCollectorMessages;
        static Mock<IAggregateRootRepositoryFor<TextMessagesFromUnknownSender>> _unknownSenderMessages;
        static Mock<IUncommittedEventStreamCoordinator> _uncommittedEventStreamCoordinator;
        static Mock<IExecutionContextManager> _executionContextManager;
        static Mock<ILogger> _logger;
        static Guid _dataCollectorId;

        Establish context = () =>
        {
            const string dataCollectorPhoneNumber = "+4790090900";
            _dataCollectorId = Guid.NewGuid();
            const string text = "this is a message";

            _logger = new Mock<ILogger>();

            _uncommittedEventStreamCoordinator = new Mock<IUncommittedEventStreamCoordinator>();
            _uncommittedEventStreamCoordinator.Setup(_ => _.Commit(
                Moq.It.IsAny<CorrelationId>(),
                Moq.It.IsAny<UncommittedEvents>())
            ).Callback<CorrelationId, UncommittedEvents>((correlationId, uncommitedEvents) => _producedEvents = uncommitedEvents);

            var commandContext = new CommandContext(
                null,
                null,
                _uncommittedEventStreamCoordinator.Object,
                _logger.Object);

            _dataCollectors = new Mock<IReadModelRepositoryFor<DataCollector>>();
            _dataCollectors
                .Setup(_ => _.Query)
                .Returns(new[] { new DataCollector(_dataCollectorId, dataCollectorPhoneNumber) }.AsQueryable());
            _dataCollectorMessages = new Mock<IAggregateRootRepositoryFor<TextMessagesFromDataCollector>>();
            _dataCollectorMessages
                .Setup(_ => _.Get(Moq.It.IsAny<EventSourceId>()))
                .Returns<EventSourceId>(esId =>
                {
                    var messages = new TextMessagesFromDataCollector(esId);
                    commandContext.RegisterForTracking(messages);
                    return messages;
                });
            _unknownSenderMessages = new Mock<IAggregateRootRepositoryFor<TextMessagesFromUnknownSender>>();

            _executionContextManager = new Mock<IExecutionContextManager>();
            _executionContextManager
                .SetupGet(_ => _.Current)
                .Returns(new ExecutionContext(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Dolittle.Execution.Environment.Development,
                    Guid.NewGuid(),
                    Claims.Empty,
                    CultureInfo.CurrentCulture));


            
            var commandContextManager = new Mock<ICommandContextManager>();
            commandContextManager
                .Setup(_ => _.GetCurrent())
                .Returns(commandContext);
            commandContextManager
                .Setup(_ => _.EstablishForCommand(Moq.It.IsAny<CommandRequest>()))
                .Returns(commandContext);

            _lookup = new DataCollectorIdentification(
                _dataCollectors.Object,
                commandContextManager.Object,
                _dataCollectorMessages.Object,
                _unknownSenderMessages.Object);


            _event = new TextMessageReceived(
                Guid.NewGuid(),
                dataCollectorPhoneNumber,
                text,
                DateTimeOffset.Now);
        };

        Because of = () =>
        {
            _lookup.Process(_event);
        };

        It should_produce_TextMessageReceivedFromUnknown_event = () =>
        {
            _producedEvents.ShouldNotBeNull();
            _producedEvents.ShouldContainOnly(
                new TextMessageReceivedFromDataCollector(
                    _event.Id,
                    _dataCollectorId,
                    _event.Text,
                    _event.Received));
        };
    }
}
