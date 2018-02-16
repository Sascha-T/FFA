using System;

namespace FFA.Database.Models
{
    public sealed class Poll
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Script { get; set; }
        public string Description { get; set; }
        public ulong CreatorId { get; set; }
        public ulong GuildId { get; set; }
        public bool Approved { get; set; } = false;
        public bool CanDeny { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
