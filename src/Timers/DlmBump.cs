using Discord;
using Discord.WebSocket;
using FFA.Common;
using FFA.Entities.FFATimer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Timers
{
    public sealed class DlmBump : FFATimer
    {
        private readonly Credentials _credentials;

        public DlmBump(IServiceProvider provider) : base(provider, Config.DLM_BUMP_TIMER)
        {
            _credentials = provider.GetRequiredService<Credentials>();
        }

        protected override Task Execute()
        {
            if (_client.ConnectionState != ConnectionState.Connected)
                return Task.CompletedTask;

            var guild = _client.GetGuild(_credentials.GuildId);
            var general = guild.TextChannels.FirstOrDefault(x => x.Name == "general");

            if (general == default(SocketTextChannel))
                return Task.CompletedTask;

            return general.SendMessageAsync("dlm!bump");
        }
    }
}
