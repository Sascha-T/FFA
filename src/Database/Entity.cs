using MongoDB.Bson;
using System;

namespace FFA.Database
{
    public abstract class Entity
    {
        public ObjectId Id { get; set; }
        public ulong GuildId { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
