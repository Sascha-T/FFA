using System;
using System.Collections.Generic;

namespace FFA.Database.Models
{
    public class Poll
    {
        public int Id { get; }
        public string Name { get; }
        public string Script { get; }
        public string Description { get; }
        public bool Approved { get; }
        public ulong CreatorId { get; }
        public List<Vote> Votes { get; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }
}
