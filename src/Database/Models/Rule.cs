using System;

namespace FFA.Database.Models
{
    public class Rule
    {
        public Rule() { }

        public Rule(ulong guildId, string content, string category, TimeSpan? maxMuteLength = null)
        {
            GuildId = guildId;
            Content = content;
            Category = category;
            MaxMuteLength = maxMuteLength;
        }

        public int Id { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public TimeSpan? MaxMuteLength { get; set; }
        public ulong GuildId { get; set; }
    }
}
