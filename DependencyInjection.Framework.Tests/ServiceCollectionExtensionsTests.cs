using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Framework;
using Domain.Framework;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DependencyInjection.Framework.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddAllEmptyQuerries()
        {
            var serviceCollection = (IServiceCollection) new ServiceCollection();
            var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "MyTestCon");

            serviceCollection.AddEventStoreFacadeDependencies(typeof(TestQuery).Assembly, connection);
            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            var querryInDi = (TestQuery) buildServiceProvider.GetService(typeof(TestQuery));
            var allowedEventsOfQuerry =
                (SubscribedEventTypes<TestQuery>) buildServiceProvider.GetService(
                    typeof(SubscribedEventTypes<TestQuery>));
            var querryHandler = buildServiceProvider.GetService<TestHandler>();
            var querryHandlers = buildServiceProvider.GetServices<IQuerryEventHandler>().ToList();

            Assert.Equal(nameof(TestQuerryCreatedEvent), allowedEventsOfQuerry[0]);
            Assert.Equal(nameof(TestQuerryNameChangedEvent), allowedEventsOfQuerry[1]);
            Assert.Equal(querryInDi, querryHandler.QueryObject);
            Assert.Equal(2, querryHandlers.Count);
        }

        [Fact]
        public void AddAllEventHandlers()
        {
            var serviceCollection = (IServiceCollection) new ServiceCollection();
            var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "MyTestCon");

            serviceCollection.AddEventStoreFacadeDependencies(typeof(TestReactiveEventHandler).Assembly, connection);
            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            var allowedEventsOfQuerry =
                (SubscribedEventTypes<TestReactiveEventHandler>) buildServiceProvider.GetService(
                    typeof(SubscribedEventTypes<TestReactiveEventHandler>));
            var eventHandler = buildServiceProvider.GetService<TestHandler>();
            var eventHandlers = buildServiceProvider.GetServices<IQuerryEventHandler>().ToList();

            Assert.Equal(nameof(TestQuerryNameChangedEvent), allowedEventsOfQuerry[0]);
            Assert.NotNull(eventHandler);
            Assert.Equal(2, eventHandlers.Count);
        }

        [Fact]
        public void AddAllReactiveEventHandlers()
        {
            var serviceCollection = (IServiceCollection) new ServiceCollection();
            var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "MyTestCon");

            serviceCollection.AddEventStoreFacadeDependencies(typeof(TestReactiveEventHandler).Assembly, connection);
            var buildServiceProvider = serviceCollection.BuildServiceProvider();

            var nameChangedHandler = buildServiceProvider.GetServices(typeof(IHandleAsync<TestQuerryNameChangedEvent>));
            var createdHandler = buildServiceProvider.GetServices(typeof(IHandleAsync<TestQuerryCreatedEvent>));

            var eventHandlerName = buildServiceProvider.GetService<HandlerDelegator<TestQuerryNameChangedEvent>>();
            var eventHandlerCreated = buildServiceProvider.GetService<HandlerDelegator<TestQuerryNameChangedEvent>>();

            Assert.Equal(2, nameChangedHandler.Count());
            Assert.Equal(1, createdHandler.Count());
            Assert.NotNull(eventHandlerName);
            Assert.NotNull(eventHandlerCreated);
        }
    }

    public class TestQuery : Query
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

    public class TestHandler : QueryEventHandler<TestQuery>
    {
        public TestHandler(TestQuery queryObject, SubscribedEventTypes<TestQuery> subscribedEventTypes) : base(
            queryObject, subscribedEventTypes)
        {
        }
    }

    public class TestReactiveEventHandler : IHandleAsync<TestQuerryNameChangedEvent>
    {
        public Task HandleAsync(TestQuerryNameChangedEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }

    public class TestReactiveEventHandler2 : IHandleAsync<TestQuerryNameChangedEvent>, IHandleAsync<TestQuerryCreatedEvent>
    {
        public Task HandleAsync(TestQuerryNameChangedEvent domainEvent)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(TestQuerryCreatedEvent domainEvent)
        {
            return Task.CompletedTask;
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