namespace FFA.Database.Models
{
    public class Vote
    {
        public int Id { get; }
        public ulong VoterId { get; }
        public bool For { get; }
        public int PollId { get; }
    }
}
