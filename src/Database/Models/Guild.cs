namespace FFA.Database.Models
{
    public sealed class Guild : Entity
    {
        public Guild() { }

        public ulong GuildId { get; set; }
        public ulong? MutedRoleId { get; set; }
        public ulong? LogChannelId { get; set; }
        public ulong? RulesChannelId { get; set; }
        public ulong? ArchiveChannelId { get; set; }
        public uint LogCase { get; set; } = 1;
        public bool AutoMute { get; set; } = true;
    }
}
