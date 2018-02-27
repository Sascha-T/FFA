using FFA.Database.Models;
using FFA.Extensions.Database;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ReputationService
    {
        private readonly IMongoCollection<User> _userCollection;

        public ReputationService(IMongoCollection<User> userCollection)
        {
            _userCollection = userCollection;
        }

        public async Task<bool> IsInTopAsync(int count, ulong userId, ulong guildId)
        {
            var result = await _userCollection.WhereAsync(x => x.GuildId == guildId);
            var topUsers = result.OrderByDescending(x => x.Reputation).Take(count);
            
            return topUsers.Any(x => x.UserId == userId);
        }
    }
}
