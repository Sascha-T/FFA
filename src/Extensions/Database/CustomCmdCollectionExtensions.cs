using FFA.Database.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace FFA.Extensions.Database
{
    public static class CustomCmdCollectionExtensions
    {
        public static Task<bool> AnyCustomCmdAsync(this IMongoCollection<CustomCmd> collection, string name, ulong guildId)
        {
            var lowerInput = name.ToLower();
            return collection.AnyAsync(x => x.Name == lowerInput && x.GuildId == guildId);
        }

        public static Task<CustomCmd> FindCustomCmdAsync(this IMongoCollection<CustomCmd> collection, string name, ulong guildId)
        {
            var lowerInput = name.ToLower();
            return collection.FindOneAsync(x => x.Name == lowerInput && x.GuildId == guildId);
        }
    }
}
