using System;

namespace FFA.Database.Models
{
    public class Poll
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Script { get; set; }
        public string Description { get; set; }
        public bool Approved { get; set; }
        public ulong CreatorId { get; set; }
        public ulong GuildId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
