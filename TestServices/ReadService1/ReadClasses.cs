using System;
using System.Threading.Tasks;
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
            Console.WriteLine($"HandleAsync2 for Event2  Working!");
            return Task.CompletedTask;
        }

        public Task HandleAsync(Event4 domainEvent)
        {
            Console.WriteLine($"HandleAsync3 for Event4  Working!");
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
            Console.WriteLine($"HandleAsync for Event2 Working!");
            return Task.CompletedTask;
        }
    }

    public class EventNotPublished : ISubscribedDomainEvent
    {
        public EventNotPublished(string entityId)
        {
            EntityId = entityId;
        }

        public string EntityId { get; }
    }

    public class ReadModel1 : ReadModel<Event2>, IHandle<Event2>, IHandle<Event4>
    {
        public void Handle(Event2 domainEvent)
        {
            Console.WriteLine($"RM Working!");
        }

        public void Handle(Event4 domainEvent)
        {
        }
    }

    public class Querry1 : Query, IHandle<Event2>
    {
        public void Handle(Event2 domainEvent)
        {
            Console.WriteLine($"Querry Working!");
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
        public Event1(string entityId)
        {
            EntityId = entityId;
        }

        public string EntityId { get; }
    }


    public class Event2 : ISubscribedDomainEvent
    {
        public Event2(string entityId)
        {
            EntityId = entityId;
        }


        public string Name { get; }
        public string SurName { get; }

        public string EntityId { get; }
    }

    public class Event3 : ISubscribedDomainEvent
    {
        public Event3(string entityId)
        {
            EntityId = entityId;
        }

        public string RmName { get; }
        public string RmSurName { get; }
        public string EntityId { get; }
    }


    public class Event4 : ISubscribedDomainEvent
    {
        public Event4(string entityId)
        {
            EntityId = entityId;
        }

        public int Age { get; }
        public string Name { get; }
        public string WeirdName { get; }

        public string EntityId { get; }
    }
}