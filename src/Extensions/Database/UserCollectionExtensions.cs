using Discord;
using FFA.Database.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Extensions.Database
{
    // TODO: proper internal vs public
    internal static class UserCollectionExtensions
    {
        public static Task<User> GetUserAsync(this IMongoCollection<User> collection, ulong userId, ulong guildId)
            => collection.GetAsync(x => x.UserId == userId && x.GuildId == guildId, x =>
            {
                x.UserId = userId;
                x.GuildId = guildId;
            });

        public static Task<User> GetUserAsync(this IMongoCollection<User> collection, IGuildUser guildUser)
            => collection.GetUserAsync(guildUser.Id, guildUser.GuildId);

        public static Task UpsertUserAsync(this IMongoCollection<User> collection, ulong userId, ulong guildId, Action<User> update)
            => collection.UpsertAsync(x => x.UserId == userId && x.GuildId == guildId, update, x =>
            {
                x.UserId = userId;
                x.GuildId = guildId;
            });

        public static Task UpsertUserAsync(this IMongoCollection<User> collection, IGuildUser guildUser, Action<User> update)
            => collection.UpsertUserAsync(guildUser.Id, guildUser.GuildId, update);
    }
}
