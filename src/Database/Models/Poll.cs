using System;
using System.Collections.Generic;

namespace FFA.Database.Models
{
    public class Poll
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Script { get; set; }
        public string Description { get; set; }
        public bool Approved { get; set; }
        public ulong CreatorId { get; set; }
        public List<Vote> Votes { get; set; } = new List<Vote>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
