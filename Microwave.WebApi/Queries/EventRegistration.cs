using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microwave.WebApi.Queries
{
    [Serializable]
    public class EventRegistration : Dictionary<string, Type>
    {
        public EventRegistration()
        {
        }

        protected EventRegistration(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}