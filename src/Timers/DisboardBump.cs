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
    public sealed class DisboardBump : FFATimer
    {
        private readonly Credentials _credentials;

        public DisboardBump(IServiceProvider provider) : base(provider, Config.DISBOARD_BUMP_TIMER)
        {
            _credentials = provider.GetRequiredService<Credentials>();
        }

        protected override async Task Execute()
        {
            var guild = _client.GetGuild(_credentials.GuildId);
            var general = guild.TextChannels.FirstOrDefault(x => x.Name == "general");

            if (general == default(SocketTextChannel) || !await general.CanSendAsync())
                return;

            await general.SendMessageAsync("Please use the command `!disboard bump` to support this server.");
        }
    }
}
