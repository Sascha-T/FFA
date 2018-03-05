namespace FFA.Database.Models.Sub
{
    public sealed class ClearAction : Action
    {
        public ulong SubjectId { get; set; }
        public ulong ChannelId { get; set; }
        public uint Quantity { get; set; }
        public string RuleContent { get; set; }
    }
}
