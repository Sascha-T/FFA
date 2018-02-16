namespace FFA.Database.Models
{
    public sealed class User
    {
        public User() { }

        public User(ulong id, ulong guildId)
        {
            Id = id;

        }

        public ulong Id { get; set; }
        public int Reputation { get; set; } = 0;
        public ulong GuildId { get; set; }
    }
}
