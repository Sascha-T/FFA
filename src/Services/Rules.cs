using Discord;
using Discord.WebSocket;
using FFA.Common;
using FFA.Database;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public class Rules
    {
        private readonly FFAContext _ffaContext;
        private readonly DiscordSocketClient _client;
        private readonly Credentials _credentials;
        private readonly Sender _sender;

        public Rules(FFAContext ffaContext, DiscordSocketClient client, Credentials credentials, Sender sender)
        {
            _ffaContext = ffaContext;
            _client = client;
            _credentials = credentials;
            _sender = sender;
        }

        public async Task Update()
        {
            var rulesChannel = _client.GetChannel(_credentials.RulesChannelId) as SocketTextChannel;
            var messages = await rulesChannel.GetMessagesAsync().FlattenAsync();
            await rulesChannel.DeleteMessagesAsync(messages);

            int i = 0;

            foreach (var group in _ffaContext.Rules.OrderBy((x) => x.Category).GroupBy((x) => x.Category))
            {
                var description = string.Empty;
                int j = 0;

                foreach (var rule in group.OrderBy((x) => x.Description))
                {
                    description += $"{(char)('a' + j++)}. {rule.Description} " +
                                   $"({(rule.MaxMuteLength.HasValue ? "Bannable" : rule.MaxMuteLength.Value.Hours + "h")})";
                }

                await _sender.SendAsync(rulesChannel, description, $"{++i}. {group.First().Category}:");
            }
        }
    }
}
