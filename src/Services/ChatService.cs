using FFA.Common;
using FFA.Database.Models;
using FFA.Entities.Service;
using FFA.Extensions.Database;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ChatService : Service
    {
        private readonly ConcurrentDictionary<ulong, DateTimeOffset> _cooldowns = new ConcurrentDictionary<ulong, DateTimeOffset>();
        private readonly IMongoCollection<User> _dbUsers;

        public ChatService(IMongoCollection<User> dbUsers)
        {
            _dbUsers = dbUsers;
        }

        public Task ApplyAsync(ulong userId, ulong guildId)
        {
            if (!_cooldowns.TryGetValue(userId, out DateTimeOffset endsAt))
            {
                _cooldowns.TryAdd(userId, GetEndsAt());
            }
            else if (endsAt.CompareTo(DateTimeOffset.UtcNow) > 0)
            {
                return Task.CompletedTask;
            }
            else
            {
                _cooldowns.TryUpdate(userId, GetEndsAt(), endsAt);
            }

            return _dbUsers.UpsertUserAsync(userId, guildId, x => x.Reputation += Config.CHAT_REWARD);
        }

        private static DateTimeOffset GetEndsAt()
            => DateTimeOffset.UtcNow.Add(Config.CHAT_SERVICE_DELAY);
    }
}
