using System;
using System.Collections.Generic;

namespace Microwave.Application.Discovery
{
    public class PublisherEventConfig
    {
        public PublisherEventConfig(Uri serviceBaseAddress,
            IEnumerable<EventSchema> publishedEventTypes,
            bool isReachable = true,
            string serviceName = null)
        {
            ServiceBaseAddress = serviceBaseAddress;
            ServiceName = serviceName ?? serviceBaseAddress.ToString();
            PublishedEventTypes = publishedEventTypes;
            IsReachable = isReachable;
        }

        public Uri ServiceBaseAddress { get; }
        public string ServiceName { get; }
        public IEnumerable<EventSchema> PublishedEventTypes { get; }
        public bool IsReachable { get; }
    }

}