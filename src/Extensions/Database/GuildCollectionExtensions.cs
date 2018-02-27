using FFA.Database.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Extensions.Database
{
    internal static class GuildCollectionExtensions
    {
        public static Task<Guild> GetGuildAsync(this IMongoCollection<Guild> collection, ulong guildId)
            => collection.GetAsync(x => x.GuildId == guildId, x => x.GuildId = guildId);

        public static Task UpsertGuildAsync(this IMongoCollection<Guild> collection, ulong guildId, Action<Guild> update)
            => collection.UpsertAsync(x => x.GuildId == guildId, update, x => x.GuildId = guildId);
    }
}
