using FFA.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public class ReputationService
    {
        private readonly FFAContext _ffaContext;

        public ReputationService(FFAContext ffaContext)
        {
            _ffaContext = ffaContext;
        }

        public async Task<bool> IsInTopAsync(int count, ulong userId, ulong guildId)
        {
            var topUsers = _ffaContext.Users.Where(x => x.GuildId == guildId).OrderByDescending(x => x.Reputation).Take(count);
            
            return await topUsers.AnyAsync(x => x.Id == userId);
        }
    }
}
