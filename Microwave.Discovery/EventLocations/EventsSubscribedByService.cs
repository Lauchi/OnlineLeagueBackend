using System.Collections.Generic;

namespace Microwave.Discovery.EventLocations
{
    public class EventsSubscribedByService
    {
        public EventsSubscribedByService(
            IEnumerable<EventSchema> events,
            IEnumerable<ReadModelSubscription> readModelSubcriptions)
        {
            Events = events;
            ReadModelSubcriptions = readModelSubcriptions;
        }

        public IEnumerable<EventSchema> Events { get; }
        public IEnumerable<ReadModelSubscription> ReadModelSubcriptions { get; }
    }
}