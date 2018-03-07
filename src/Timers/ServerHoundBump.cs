using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using FFA.Common;
using FFA.Entities.FFATimer;
using FFA.Extensions.Discord;
using Microsoft.Extensions.DependencyInjection;

namespace FFA.Timers
{
    public sealed class ServerHoundBump : FFATimer
    {
        private readonly Credentials _credentials;

        public ServerHoundBump(IServiceProvider provider) : base(provider, Config.SERVER_HOUND_BUMP_TIMER)
        {
            _credentials = provider.GetRequiredService<Credentials>();
        }

        protected override async Task Execute()
        {
            // TODO: extension to get general channel
            var guild = _client.GetGuild(_credentials.GuildId);
            var general = guild.TextChannels.FirstOrDefault(x => x.Name == "general");

            if (general == default(SocketTextChannel) || !await general.CanSendAsync())
                return;

            await general.SendMessageAsync("=bump");
        }
    }
}
