namespace FFA.Database.Models
{
    public sealed class Rule : Entity
    {
        public Rule() { }

        public Rule(ulong guildId, string content, string category, uint? maxMuteHours = null)
        {
            GuildId = guildId;
            Content = content;
            Category = category;
            MaxMuteHours = maxMuteHours;
        }
        
        public string Content { get; set; }
        public string Category { get; set; }
        public uint? MaxMuteHours { get; set; }
        public ulong GuildId { get; set; }
    }
}
