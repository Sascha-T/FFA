using FFA.Database;
using System.Linq;

namespace FFA.Services
{
    public class ReputationService
    {
        private readonly FFAContext _ffaContext;

        public ReputationService(FFAContext ffaContext)
        {
            _ffaContext = ffaContext;
        }

        public bool IsInTop(int count, ulong userId, ulong guildId)
        {
            var topUsers = _ffaContext.Users.Where(x => x.GuildId == guildId).OrderByDescending(x => x.Reputation).Take(count);

            return topUsers.Any(x => x.Id == userId);
        }
    }
}
