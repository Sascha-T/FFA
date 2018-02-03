using Discord;
using Discord.WebSocket;
using FFA.Database;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public class RulesService
    {
        private readonly FFAContext _ffaContext;
        private readonly SendingService _sender;

        public RulesService(FFAContext ffaContext, SendingService sender)
        {
            _ffaContext = ffaContext;
            _sender = sender;
        }

        public async Task UpdateAsync(IGuild guild)
        {
            var dbGuild = await _ffaContext.GetGuildAsync(guild.Id);

            if (dbGuild.RulesChannelId.HasValue)
            {
                var rulesChannel = await guild.GetChannelAsync(dbGuild.RulesChannelId.Value) as SocketTextChannel;
                var messages = await rulesChannel.GetMessagesAsync().FlattenAsync();
                await rulesChannel.DeleteMessagesAsync(messages);

                int i = 0;

                foreach (var group in _ffaContext.Rules.Where(x => x.GuildId == guild.Id).OrderBy(x => x.Category).GroupBy(x => x.Category))
                {
                    int j = 0;
                    var description = string.Empty;

                    foreach (var rule in group.OrderBy(x => x.Content))
                    {
                        description += $"**{(char)('a' + j++)}.** {rule.Content} " +
                                       $"({(rule.MaxMuteLength.HasValue ? rule.MaxMuteLength.Value.TotalHours + "h" : "Bannable")})\n";
                    }

                    await _sender.SendAsync(rulesChannel, description, $"{++i}. {group.First().Category}:");
                }
            }
        }
    }
}
