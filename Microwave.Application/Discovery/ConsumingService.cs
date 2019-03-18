using System;
using System.Collections.Generic;

namespace Microwave.Application.Discovery
{
    public class ConsumingService
    {
        public ConsumingService(Uri serviceBaseAddress, IEnumerable<string> publishedEventTypes, string
            serviceName = null)
        {
            ServiceBaseAddress = serviceBaseAddress;
            ServiceName = serviceName ?? serviceBaseAddress.ToString();
            PublishedEventTypes = publishedEventTypes;
        }

        public Uri ServiceBaseAddress { get; }
        public string ServiceName { get; }
        public IEnumerable<string> PublishedEventTypes { get; }
    }
}