using Discord;
using Discord.WebSocket;
using FFA.Database;
using Microsoft.EntityFrameworkCore;
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
                var groups = await _ffaContext.Rules.Where(x => x.GuildId == guild.Id).OrderBy(x => x.Category).GroupBy(x => x.Category).ToListAsync();

                for (var i = 0; i < groups.Count; i++)
                {
                    var description = string.Empty;
                    var j = 0;

                    foreach (var rule in groups[i].OrderBy(x => x.Content))
                    {
                        description += $"**{(char)('a' + j++)}.** {rule.Content} " +
                                       $"({(rule.MaxMuteLength.HasValue ? rule.MaxMuteLength.Value.TotalHours + "h" : "Bannable")})\n";
                    }

                    await _sender.SendAsync(rulesChannel, description, $"{i}. {groups[0].First().Category}:");
                    await Task.Delay(1000);
                }
            }
        }
    }
}
