using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Adapters.Framework.EventStores;
using Application.Framework;
using Domain.Framework;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DependencyInjection.Framework.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public async Task AddAllLoadedQuerries()
        {
            var eventStoreConnection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "MyTestCon");
            await eventStoreConnection.ConnectAsync();
            await eventStoreConnection.DeleteStreamAsync(new TestEventStoreConfig().EventStream, ExpectedVersion.Any,
                new UserCredentials("admin", "changeit"));
            var eventStore = new EventStoreFacade(new EventSourcingApplyStrategy(), eventStoreConnection, new TestEventStoreConfig());
            var entityId = Guid.NewGuid();
            var domainEvents = new List<DomainEvent>
            {
                new TestQuerryCreatedEvent(entityId, "NameFirst"),
                new TestQuerryNameChangedEvent(entityId, "NameSecond")
            };

            await eventStore.AppendAsync(domainEvents);

            var serviceCollection = (IServiceCollection) new ServiceCollection();
            serviceCollection.AddAllLoadedQuerries(typeof(TestQuerry).Assembly, eventStore);
            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            var querryInDi = (TestQuerry) buildServiceProvider.GetService(typeof(TestQuerry));

            Assert.Equal("NameSecond", querryInDi.Name);
        }
    }

    public class TestQuerry : Querry
    {
        public string Name { get; private set; }

        public void Apply(TestQuerryCreatedEvent domainEvent)
        {
            Name = domainEvent.Name;
        }

        public void Apply(TestQuerryNameChangedEvent domainEvent)
        {
            Name = domainEvent.Name;
        }
    }

    public class TestQuerryNameChangedEvent : DomainEvent
    {
        public TestQuerryNameChangedEvent(Guid entityId, string name) : base(entityId)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class TestQuerryCreatedEvent : DomainEvent
    {
        public TestQuerryCreatedEvent(Guid entityId, string name) : base(entityId)
        {
            Name = name;
        }

        public string Name { get; }
    }
}