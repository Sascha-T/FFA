using System;

// TODO: proper bson date type
namespace FFA.Database.Models
{
    // TODO: make unmuting remove ALL the mutes under that person's id and guild id.
    public sealed class Mute : Entity
    {
        public Mute() { }

        public Mute(ulong guildId, ulong userId, TimeSpan length)
        {
            GuildId = guildId;
            UserId = userId;
            EndsAt = DateTime.UtcNow.Add(length);
        }

        public ulong UserId { get; set; }
        public DateTime EndsAt { get; set; }
        public ulong GuildId { get; set; }
    }
}
