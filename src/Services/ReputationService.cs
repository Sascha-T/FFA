using FFA.Database.Models;
using FFA.Entities.Service;
using FFA.Extensions.Database;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ReputationService : Service
    {
        private readonly IMongoCollection<User> _dbUsers;

        public ReputationService(IMongoCollection<User> dbUsers)
        {
            _dbUsers = dbUsers;
        }

        public async Task<bool> IsInTopAsync(int count, ulong userId, ulong guildId)
        {
            var result = await _dbUsers.WhereAsync(x => x.GuildId == guildId);
            var topUsers = result.OrderByDescending(x => x.Reputation).Take(count);

            return topUsers.Any(x => x.UserId == userId);
        }
    }
}
