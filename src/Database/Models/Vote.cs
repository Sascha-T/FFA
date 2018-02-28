using System;

namespace FFA.Database.Models
{
    // TODO: consistency: DateTime vs DateTimeOffset
    public sealed class Vote : Entity
    {
        public Vote() { }

        public bool For { get; set; }
        public int PollId { get; set; }
        public ulong VoterId { get; set; }
        public ulong GuildId { get; set; }
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}
