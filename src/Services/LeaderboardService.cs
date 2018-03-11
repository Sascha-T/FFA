using Discord;
using FFA.Database.Models;
using FFA.Entities.Service;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Extensions.System;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class LeaderboardService : Service
    {
        private readonly IMongoCollection<User> _dbUsers;
        private readonly IMongoCollection<CustomCmd> _dbCustomCmds;

        public LeaderboardService(IMongoCollection<User> dbUsers, IMongoCollection<CustomCmd> dbCustomCmds)
        {
            _dbUsers = dbUsers;
            _dbCustomCmds = dbCustomCmds;
        }
        
        public async Task<string> GetUserLbAsync<TKey>(IGuild guild, Func<User, TKey> keySelector, int quantity, bool ascending = false)
        {
            var dbGuildUsers = await _dbUsers.WhereAsync(x => x.GuildId == guild.Id);
            var ordered = ascending ? dbGuildUsers.OrderBy(keySelector) : dbGuildUsers.OrderByDescending(keySelector);
            var orderedArr = ordered.ToArray();
            var descBuilder = new StringBuilder();
            var pos = 0;

            for (int i = 0; i < orderedArr.Length; i++)
            {
                var user = await guild.GetUserAsync(orderedArr[i].UserId);

                if (user != null)
                    descBuilder.AppendFormat("{0}. {1}: {2}\n", ++pos, user.Bold(), orderedArr[i].Reputation.ToString("F2"));

                if (pos == quantity)
                    break;
            }

            return descBuilder.ToString();
        }

        public async Task<string> GetCustomCmdsAsync<TKey>(ulong guildId, Func<CustomCmd, TKey> keySelector, int quantity, bool ascending = false)
        {
            var dbGuildCmds = await _dbCustomCmds.WhereAsync(x => x.GuildId == guildId);
            var ordered = ascending ? dbGuildCmds.OrderBy(keySelector) : dbGuildCmds.OrderByDescending(keySelector);
            var OrderedArr = ordered.ToArray();
            var desc = string.Empty;
            var pos = 0;

            for (int i = 0; i < OrderedArr.Length; i++)
            {
                if (OrderedArr[i].Uses > 0)
                    desc += $"{(++pos)}. {OrderedArr[i].Name.UpperFirstChar().Bold()}: {OrderedArr[i].Uses}\n";

                if (pos == quantity)
                    break;
            }

            return desc;
        }
    }
}
