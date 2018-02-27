using FFA.Common;
using System;

namespace FFA.Database.Models
{
    // TODO: make unmuting remove ALL the mutes under that person's id and guild id.
    public sealed class Mute : Entity
    {
        public Mute() { }

        public Mute(ulong guildId, ulong userId, uint hours)
        {
            GuildId = guildId;
            UserId = userId;
            EndsAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (hours * Configuration.MS_PER_HOUR);
        }
        
        public ulong UserId { get; set; }
        public long EndsAt { get; set; }
        public ulong GuildId { get; set; }
    }
}
