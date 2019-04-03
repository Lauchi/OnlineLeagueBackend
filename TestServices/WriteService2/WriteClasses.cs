using System.Threading.Tasks;
using Microwave.Domain;
using Microwave.Queries;

namespace WriteService2
{
    public class Handler1 : IHandleAsync<Event2>
    {
        public Task HandleAsync(Event2 domainEvent)
        {
            return null;
        }
    }
    public class Entity2 : Entity, IApply<Event3>, IApply<Event4>
    {
        public void Apply(Event3 domainEvent)
        {
        }

        public void Apply(Event4 domainEvent)
        {
        }
    }
    public class Event3 : IDomainEvent
    {
        public Event3(Identity entityId)
        {
            EntityId = entityId;
        }

        public Identity EntityId { get; }
    }


    public class Event4 : IDomainEvent
    {
        public Event4(Identity entityId)
        {
            EntityId = entityId;
        }

        public Identity EntityId { get; }
    }
    
    public class Event2 : IDomainEvent
    {
        public Event2(Identity entityId)
        {
            EntityId = entityId;
        }

        public Identity EntityId { get; }
    }
}