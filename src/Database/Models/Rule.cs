using System;

namespace FFA.Database.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool Bannable { get; set; }
        public TimeSpan MaxMuteLength { get; set; }
    }
}
