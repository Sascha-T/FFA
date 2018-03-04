using FFA.Database.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Extensions.Database
{
    public static class GuildCollectionExtensions
    {
        private static UpdateDefinition<Guild> GetFactory(ulong guildId)
            => new UpdateDefinitionBuilder<Guild>()
            .SetOnInsert(x => x.GuildId, guildId)
            .SetOnInsert(x => x.LogCase, 1u)
            .SetOnInsert(x => x.AutoMute, true);

        public static Task<Guild> GetGuildAsync(this IMongoCollection<Guild> collection, ulong guildId)
            => collection.GetAsync(x => x.GuildId == guildId, GetFactory(guildId));

        public static Task UpsertGuildAsync(this IMongoCollection<Guild> collection, ulong guildId, Action<Guild> update)
            => collection.UpsertAsync(x => x.GuildId == guildId, update, GetFactory(guildId));
    }
}
