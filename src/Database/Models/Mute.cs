using System;

namespace FFA.Database.Models
{
    // TODO: make unmuting remove ALL the mutes under that person's id and guild id.
    public class Mute
    {
        public Mute() { }

        public Mute(ulong guildId, ulong userId, DateTimeOffset endsAt)
        {
            GuildId = guildId;
            UserId = userId;
            EndsAt = endsAt;
        }

        public int Id { get; set; }
        public ulong UserId { get; set; }
        public DateTimeOffset EndsAt { get; set; }
        public ulong GuildId { get; set; }
    }
}
