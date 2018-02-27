namespace FFA.Database.Models
{
    // TODO: role/channel deletion event to nullify the roles + channels
    public sealed class Guild : Entity
    {
        public Guild() { }

        public ulong GuildId { get; set; }
        public ulong? MutedRoleId { get; set; }
        public ulong? LogChannelId { get; set; }
        public ulong? RulesChannelId { get; set; }
        public ulong? ArchiveChannelId { get; set; }
        public uint LogCase { get; set; } = 1;
    }
}
