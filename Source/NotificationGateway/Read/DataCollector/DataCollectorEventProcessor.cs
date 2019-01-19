using Concepts.DataCollector;
using Dolittle.Artifacts;
using Dolittle.Events.Processing;
using Dolittle.ReadModels;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Events.DataCollectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Read.DataCollector
{
    class DataCollectorEventProcessor : ICanProcessEvents
    {
        private readonly IReadModelRepositoryFor<DataCollector> _dataCollectors;

        public DataCollectorEventProcessor(IReadModelRepositoryFor<DataCollector> dataCollectors)
        {
            _dataCollectors = dataCollectors;
        }

        public void Process(PhoneNumberAddedToDataCollector @event)
        {
            _dataCollectors.Insert(new DataCollector(
                @event.DataCollectorId,
                @event.PhoneNumber
            ));
        }

        public void Process(PhoneNumberRemovedFromDataCollector @event)
        {
            var dataCollector = _dataCollectors.GetById(@event.DataCollectorId);
            _dataCollectors.Delete(dataCollector);
        }
    }
}
