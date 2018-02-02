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

        public int Id { get; set; }
        public ulong UserId { get; set; }
        public DateTime EndsAt { get; set; }
    }
}
