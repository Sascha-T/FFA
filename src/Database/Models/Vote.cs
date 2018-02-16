namespace FFA.Database.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public ulong VoterId { get; set; }
        public bool For { get; set; }
        public int PollId { get; set; }
        public Poll Poll { get; set; }
    }
}
