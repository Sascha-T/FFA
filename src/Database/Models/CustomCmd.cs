namespace FFA.Database.Models
{
    public sealed class CustomCmd : Entity
    {
        public CustomCmd() { }

        public CustomCmd(ulong ownerId, ulong guildId, string name, string response)
        {
            OwnerId = ownerId;
            GuildId = guildId;
            Name = name;
            Response = response;
        }

        public ulong OwnerId { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Response { get; set; }
    }
}
