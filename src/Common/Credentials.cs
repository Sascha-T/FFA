namespace FFA.Common
{
    public class Credentials
    {
        public string Token { get; set; }
        public ulong[] OwnerIds { get; set; }
        public ulong MutedRoleId { get; set; }
        public ulong GuildId { get; set; }
        public ulong ModLogChannelId { get; set; }
        public ulong RulesChannelId { get; set; }
    }
}
