using Discord;
using FFA.Database;
using FFA.Database.Models;
using FFA.Entities.Service;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Extensions.System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class LeaderboardService : Service
    {
        private readonly IServiceProvider _provider;

        public LeaderboardService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<string> GetLbAsync<T, TKey>(IGuild guild, Func<T, TKey> keySelector,
            Func<T, Task<ValueTuple<bool, string>>> formatter, int quantity, bool ascending = false)
            where T : Entity
        {
            var collection = _provider.GetRequiredService<IMongoCollection<T>>();
            var elements = await collection.WhereAsync(x => x.GuildId == guild.Id);
            var ordered = ascending ? elements.OrderBy(keySelector) : elements.OrderByDescending(keySelector);
            var orderedArr = ordered.ToArray();
            var descBuilder = new StringBuilder();
            var pos = 0;

            for (int i = 0; i < orderedArr.Length; i++)
            {
                var (success, msg) = await formatter(orderedArr[i]);

                if (success)
                    descBuilder.AppendFormat("{0}. {1}\n", ++pos, msg);

                if (pos == quantity)
                    break;
            }

            return descBuilder.ToString();
        }

        public Task<string> GetUserLbAsync<TKey>(IGuild guild, Func<User, TKey> keySelector, int quantity,
            bool ascending = false)
            => GetLbAsync(guild, keySelector, async (dbUser) => {
                var user = await guild.GetUserAsync(dbUser.UserId);

                if (user == null)
                    return (false, string.Empty);

                return (true, $"{user.Bold()}: {dbUser.Reputation.ToString("F2")}");
            }, quantity, ascending);
        
        public Task<string> GetCustomCmdsAsync<TKey>(IGuild guild, Func<CustomCmd, TKey> keySelector,
            int quantity, bool ascending = false)
            => GetLbAsync(guild, keySelector, (dbCmd) => {
                if (dbCmd.Uses == 0)
                    return Task.FromResult((false, string.Empty));

                return Task.FromResult((true, $"{dbCmd.Name.UpperFirstChar().Bold()}: {dbCmd.Uses}"));
            }, quantity, ascending);
    }
}
