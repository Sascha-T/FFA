namespace FFA.Database.Models
{
    public sealed class User : Entity
    {
        public User() { }

        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public int Reputation { get; set; } = 0;
    }
}
