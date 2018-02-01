namespace FFA.Common
{
    public class Credentials
    {
        public string Token { get; set; }

        public ulong[] OwnerIds { get; set; }

        public ulong MutedRoleId { get; }

        public ulong ModLogChannelId { get; }
    }
}
