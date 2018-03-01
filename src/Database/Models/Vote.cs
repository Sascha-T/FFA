namespace FFA.Database.Models
{
    public sealed class Vote : Entity
    {
        public bool For { get; set; }
        public int PollId { get; set; }
        public ulong VoterId { get; set; }
        public ulong GuildId { get; set; }
    }
}
