using Discord;
using FFA.Database;
using FFA.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class RulesService
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

            if (!dbGuild.RulesChannelId.HasValue)
            {
                return;
            }

            var rulesChannel = await guild.GetChannelAsync(dbGuild.RulesChannelId.Value) as ITextChannel;

            if (rulesChannel == null || !await rulesChannel.CanSendAsync())
            {
                return;
            }

            var messages = await rulesChannel.GetMessagesAsync().FlattenAsync();
            await rulesChannel.DeleteMessagesAsync(messages);
            var groups = await _ffaContext.Rules.Where(x => x.GuildId == guild.Id).OrderBy(x => x.Category).GroupBy(x => x.Category).ToListAsync();

            for (var i = 0; i < groups.Count; i++)
            {
                var description = string.Empty;

                foreach (var item in groups[i].OrderBy(x => x.Content).Select((Value, Index) => new { Value, Index }))
                {
                    description += $"**{(char)('a' + item.Index)}.** {item.Value.Content} " +
                                   $"({(item.Value.MaxMuteLength.HasValue ? item.Value.MaxMuteLength.Value.TotalHours + "h" : "Bannable")})\n";
                }

                await _sender.SendAsync(rulesChannel, description, $"{i + 1}. {groups[i].First().Category}:");
                await Task.Delay(1000);
            }
        }
    }
}
