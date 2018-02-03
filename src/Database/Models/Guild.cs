namespace FFA.Database.Models
{
    // TODO: role/channel deletion event to nullify the roles + channels
    public class Guild
    {
        public ulong Id { get; set; }
        public ulong? MutedRoleId { get; set; }    
        public ulong? LogChannelId { get; set; }
        public ulong? RulesChannelId { get; set; }
        public uint LogCase { get; set; } = 1;
    }
}
