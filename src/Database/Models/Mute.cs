using System;

namespace FFA.Database.Models
{
    // TODO: make unmuting remove ALL the mutes under that person's id and guild id.
    public sealed class Mute : Entity
    {
        public Mute(ulong guildId, ulong userId, TimeSpan length)
        {
            GuildId = guildId;
            UserId = userId;
            Length = length;
        }

        public ulong UserId { get; set; }
        public TimeSpan Length { get; set; }
        public ulong GuildId { get; set; }
    }
}
