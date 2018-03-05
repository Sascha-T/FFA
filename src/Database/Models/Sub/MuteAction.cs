using MongoDB.Bson;

namespace FFA.Database.Models.Sub
{
    public sealed class MuteAction : Action
    {
        public ObjectId MuteId { get; set; }
        public string RuleContent { get; set; }
    }
}
