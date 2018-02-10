using FFA.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ReputationService
    {
        public Task<bool> IsInTopAsync(FFAContext ffaContext, int count, ulong userId, ulong guildId)
        {
            var topUsers = ffaContext.Users.Where(x => x.GuildId == guildId).OrderByDescending(x => x.Reputation).Take(count);

            return topUsers.AnyAsync(x => x.Id == userId);
        }
    }
}
