using Concepts.DataCollector;
using Concepts.SMS;
using Dolittle.ReadModels;

namespace Read.DataCollector
{
    public class DataCollector : IReadModel
    {
        public DataCollector(DataCollectorId id, PhoneNumber phoneNumber)
        {
            Id = id;
            PhoneNumber = phoneNumber;
        }

        public DataCollectorId Id { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
    }
}
