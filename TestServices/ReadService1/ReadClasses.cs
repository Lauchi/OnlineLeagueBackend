using System;
using System.Threading.Tasks;
using Microwave.Domain.Identities;
using Microwave.Queries;

namespace ReadService1
{
    public class Handler1 :
        IHandleAsync<Event2>,
        IHandleAsync<Event4>,
        IHandleAsync<EventNotPublished>
    {
        public Task HandleAsync(Event2 domainEvent)
        {
            Console.WriteLine($"{DateTime.UtcNow.Second} Event2 was handled in Fast Handler");
            return Task.CompletedTask;
        }

        public Task HandleAsync(Event4 domainEvent)
        {
            Console.WriteLine("Event4 was handled");
            return Task.CompletedTask;
        }

        public Task HandleAsync(EventNotPublished domainEvent)
        {
            return Task.CompletedTask;
        }
    }

    public class Handler2 :
        IHandleAsync<Event2>
    {
        public Task HandleAsync(Event2 domainEvent)
        {
            Console.WriteLine($"{DateTime.UtcNow.Second} Event2 was handled in Sloooooow Handler 10 secs");
            return Task.CompletedTask;
        }
    }

    public class EventNotPublished : ISubscribedDomainEvent
    {
        public EventNotPublished(Identity entityId)
        {
            EntityId = entityId;
        }

        public Identity EntityId { get; }
    }

    public class ReadModel1 : ReadModel<Event2>, IHandle<Event2>, IHandle<Event4>
    {
        public void Handle(Event2 domainEvent)
        {
            Console.WriteLine($"{DateTime.UtcNow.Second} jeah 25 secs");
        }

        public void Handle(Event4 domainEvent)
        {
        }
    }

    public class Querry1 : Query, IHandle<Event2>
    {
        public void Handle(Event2 domainEvent)
        {
            Counter += 1;
        }

        public int Counter { get; set; }
    }

    public class ReadModelNotPublished : ReadModel<EventNotPublished>, IHandle<EventNotPublished>
    {
        public void Handle(EventNotPublished domainEvent)
        {
        }
    }

    public class Event1 : ISubscribedDomainEvent
    {
        public Event1(Identity entityId)
        {
            EntityId = entityId;
        }

        public Identity EntityId { get; }
    }


    public class Event2 : ISubscribedDomainEvent
    {
        public Event2(Identity entityId)
        {
            EntityId = entityId;
        }


        public string Name { get; }
        public string SurName { get; }

        public Identity EntityId { get; }
    }

    public class Event3 : ISubscribedDomainEvent
    {
        public Event3(Identity entityId)
        {
            EntityId = entityId;
        }

        public string RmName { get; }
        public string RmSurName { get; }
        public Identity EntityId { get; }
    }


    public class Event4 : ISubscribedDomainEvent
    {
        public Event4(Identity entityId)
        {
            EntityId = entityId;
        }

        public int Age { get; }
        public string Name { get; }
        public string WeirdName { get; }

        public Identity EntityId { get; }
    }
}