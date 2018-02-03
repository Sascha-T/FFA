using System;

namespace FFA.Database.Models
{
    public class Mute
    {
        public Mute() { }

        public Mute(ulong guildId, ulong userId, DateTime endsAt)
        {
            GuildId = guildId;
            UserId = userId;
            EndsAt = endsAt;
        }

        public int Id { get; set; }
        public ulong UserId { get; set; }
        public DateTime EndsAt { get; set; }
        public ulong GuildId { get; set; }
    }
}
