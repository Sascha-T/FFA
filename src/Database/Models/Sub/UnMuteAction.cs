using MongoDB.Bson;

namespace FFA.Database.Models.Sub
{
    public sealed class UnMuteAction : Action
    {
        public ObjectId MuteId { get; set; }
    }
}
