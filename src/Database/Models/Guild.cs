namespace FFA.Database.Models
{
    public sealed class Guild : Entity
    {
        public ulong? MutedRoleId { get; set; }
        public ulong? LogChannelId { get; set; }
        public ulong? RulesChannelId { get; set; }
        public ulong? ArchiveChannelId { get; set; }
        public uint LogCase { get; set; } // TODO: Rename to CaseCount
        public bool AutoMute { get; set; }
    }
}
