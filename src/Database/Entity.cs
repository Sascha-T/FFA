using MongoDB.Bson;
using System;

namespace FFA.Database
{
    public class Entity
    {
        public ObjectId Id { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
