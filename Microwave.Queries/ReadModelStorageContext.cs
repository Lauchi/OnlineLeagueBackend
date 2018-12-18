﻿using MongoDB.Bson.Serialization.Attributes;

namespace Microwave.Queries
{
    public class IdentifiableQueryDbo<T>
    {
        [BsonId]
        public string Id { get; set; }
        public T Payload { get; set; }
        public long Version { get; set; }
        public string QueryType { get; set; }
    }

    public class QueryDbo<T>
    {
        [BsonId]
        public string Type { get; set; }
        public T Payload { get; set; }
    }

    public class LastProcessedVersionDbo
    {
        [BsonId]
        public string EventType { get; set; }
        public long LastVersion { get; set; }
    }
}