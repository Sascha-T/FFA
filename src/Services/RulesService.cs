using Discord;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using MongoDB.Driver;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class RulesService
    {
        private readonly SendingService _sender;
        private readonly SemaphoreSlim _semaphore;
        private readonly IMongoCollection<Guild> _guildCollection;
        private readonly IMongoCollection<Rule> _ruleCollection;

        public RulesService(SendingService sender, IMongoCollection<Guild> guildCollection, IMongoCollection<Rule> ruleCollection)
        {
            _sender = sender;
            _guildCollection = guildCollection;
            _ruleCollection = ruleCollection;
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task UpdateAsync(IGuild guild)
        {
            await _semaphore.WaitAsync();

            try
            {
                var dbGuild = await _guildCollection.GetGuildAsync(guild.Id);

                if (!dbGuild.RulesChannelId.HasValue)
                    return;

                var rulesChannel = await guild.GetChannelAsync(dbGuild.RulesChannelId.Value) as ITextChannel;

                if (rulesChannel == null || !await rulesChannel.CanSendAsync())
                    return;

                var messages = await rulesChannel.GetMessagesAsync().FlattenAsync();
                await rulesChannel.DeleteMessagesAsync(messages);

                var result = await _ruleCollection.WhereAsync(x => x.GuildId == guild.Id);
                var groups = result.OrderBy(x => x.Category).GroupBy(x => x.Category).ToArray();

                for (var i = 0; i < groups.Length; i++)
                {
                    var description = string.Empty;

                    foreach (var item in groups[i].OrderBy(x => x.Content).Select((Value, Index) => new { Value, Index }))
                        description += $"**{(char)('a' + item.Index)}.** {item.Value.Content} " +
                                       $"({(item.Value.MaxMuteHours.HasValue ? item.Value.MaxMuteHours.Value + "h" : "Bannable")})\n";

                    await _sender.SendAsync(rulesChannel, description, $"{i + 1}. {groups[i].First().Category}:");
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
