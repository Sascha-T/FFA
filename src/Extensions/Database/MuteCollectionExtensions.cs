using FFA.Database.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace FFA.Extensions.Database
{
    public static class MuteCollectionExtensions
    {
        public static Task<bool> AnyMuteAsync(this IMongoCollection<Mute> collection, ulong userId, ulong guildId)
            => collection.AnyAsync(x => x.UserId == userId && x.GuildId == guildId && x.Active);
    }
}
