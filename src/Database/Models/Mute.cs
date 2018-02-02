using System;

namespace FFA.Database.Models
{
    public class Mute
    {
        public Mute() { }

        public Mute(ulong userId, DateTime endsAt)
        {
            UserId = userId;
            EndsAt = endsAt;
        }

        public ulong UserId { get; }
        public DateTime EndsAt { get; set; }
    }
}
