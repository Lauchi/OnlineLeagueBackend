using System;
using System.Collections.Generic;
using System.Linq;
using Microwave.Discovery.Domain.Events;
using Microwave.Discovery.Domain.Services;

namespace Microwave.Discovery.Domain
{
    public class EventLocation : IEventLocation
    {
        public IEnumerable<ServiceNode> Services { get; private set; }
            = new List<ServiceNode>();
        public IEnumerable<EventSchema> UnresolvedEventSubscriptions { get; }
        public IEnumerable<ReadModelSubscription> UnresolvedReadModeSubscriptions { get; }

        public EventLocation(IEnumerable<EventsPublishedByService> allServices, EventsSubscribedByService eventsSubscribedByService)
        {
            var readModels = eventsSubscribedByService.ReadModelSubcriptions.ToList();
            var handleAsyncEvents = eventsSubscribedByService.Events.ToList();
            UnresolvedEventSubscriptions = handleAsyncEvents;
            UnresolvedReadModeSubscriptions = readModels;

            foreach (var service in allServices)
            {
                var relevantEventsOfService = handleAsyncEvents.Where(ev => service.PublishedEventTypes.Contains(ev)).ToList();
                var relevantEvents = relevantEventsOfService.Select(relevantEvent =>
                {
                    var notFoundProperties = GetDiffOfProperties(relevantEvent, service);
                    return new EventSchema(relevantEvent.Name, notFoundProperties);
                }).ToList();

                var relevantReadModelsService = readModels.Where(r => service.PublishedEventTypes.Contains(r.GetsCreatedOn)).ToList();
                var relevantReadModels = relevantReadModelsService.Select(readModel =>
                {
                    var createdEvent = readModel.GetsCreatedOn;
                    var notFoundProperties = GetDiffOfProperties(createdEvent, service);

                    return new ReadModelSubscription(
                        readModel.ReadModelName,
                        new EventSchema(createdEvent.Name, notFoundProperties));
                }).ToList();

                if (!relevantEvents.Any() && !relevantReadModels.Any()) continue;

                SetDomainEventLocation(new ServiceNode(
                    service.ServiceEndPoint,
                    relevantEvents,
                    relevantReadModels));

                UnresolvedEventSubscriptions = UnresolvedEventSubscriptions.Except(relevantEvents);
                UnresolvedReadModeSubscriptions = UnresolvedReadModeSubscriptions.Except(relevantReadModels);
            }
        }

        private static List<PropertyType> GetDiffOfProperties(EventSchema relevantEvent, EventsPublishedByService service)
        {
            var eventSchemata = service.PublishedEventTypes.Single(ev => ev.Equals(relevantEvent));

            var foundProperties = relevantEvent.Properties.Where(p => eventSchemata.Properties.Contains(p));
            var notFoundProperties = relevantEvent.Properties.Where(p => !eventSchemata.Properties.Contains(p)).ToList();

            var allProps = foundProperties.Select(p => new PropertyType(p.Name, p.Type, true)).ToList();
            notFoundProperties.AddRange(allProps);
            return notFoundProperties;
        }

        public EventLocation(
            IEnumerable<ServiceNode> services,
            IEnumerable<EventSchema> unresolvedEventSubscriptions,
            IEnumerable<ReadModelSubscription> unresolvedReadModeSubscriptions)
        {
            Services = services;
            UnresolvedEventSubscriptions = unresolvedEventSubscriptions;
            UnresolvedReadModeSubscriptions = unresolvedReadModeSubscriptions;
        }

        public ServiceNode GetServiceForEvent(Type eventType)
        {
            return Services.FirstOrDefault(s => s.SubscribedEvents.Any(e => e.Name == eventType.Name));
        }

        public ServiceNode GetServiceForReadModel(Type readModel)
        {
            return Services.FirstOrDefault(s => s.ReadModels.Any(rm => rm.ReadModelName == readModel.Name));
        }

        private void SetDomainEventLocation(ServiceNode serviceNode)
        {
            var servicesWithoutAddedService = Services.Where(s => s.ServiceEndPoint.ServiceBaseAddress != serviceNode.ServiceEndPoint.ServiceBaseAddress);
            var services = servicesWithoutAddedService.Append(serviceNode);
            Services = services;
        }
    }
}