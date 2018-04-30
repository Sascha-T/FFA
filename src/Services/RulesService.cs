using Discord;
using FFA.Database.Models;
using FFA.Entities.Service;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using MongoDB.Driver;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class RulesService : Service
    {
        private readonly SendingService _sender;
        private readonly SemaphoreSlim _semaphore;
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<Rule> _dbRules;

        public RulesService(SendingService sender, IMongoCollection<Guild> dbGuilds, IMongoCollection<Rule> dbRules)
        {
            _sender = sender;
            _dbGuilds = dbGuilds;
            _dbRules = dbRules;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        // TODO: accept context to not refetch db guild?
        public async Task UpdateAsync(IGuild guild)
        {
            await _semaphore.WaitAsync();

            try
            {
                var dbGuild = await _dbGuilds.GetGuildAsync(guild.Id);

                if (!dbGuild.RulesChannelId.HasValue)
                    return;

                var rulesChannel = await guild.GetTextChannelAsync(dbGuild.RulesChannelId.Value);

                if (rulesChannel == null || !await rulesChannel.CanSendAsync())
                    return;

                var messages = await rulesChannel.GetMessagesAsync().FlattenAsync();
                await rulesChannel.DeleteMessagesAsync(messages);

                var result = await _dbRules.WhereAsync(x => x.GuildId == guild.Id);
                var groups = result.OrderBy(x => x.Category).GroupBy(x => x.Category).ToArray();

                // TODO: gen unique random colors to prevent duplicates?
                for (var i = 0; i < groups.Length; i++)
                {
                    var descBuilder = new StringBuilder();

                    foreach (var item in groups[i].OrderBy(x => x.Content).Select((Value, Index) => new { Value, Index }))
                        descBuilder.AppendFormat("**{0}.** {1} ({2})\n", (char)('a' + item.Index), item.Value.Content,
                            item.Value.MaxMuteLength.HasValue ? item.Value.MaxMuteLength.Value.TotalHours + "h" : "Bannable");
        
                    await _sender.SendAsync(rulesChannel, descBuilder.ToString(), $"{i + 1}. {groups[i].First().Category}:");
                    await Task.Delay(1000);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
