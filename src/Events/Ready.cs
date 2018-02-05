using Discord.WebSocket;
using FFA.Common;
using FFA.Timers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class Ready
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;

        public Ready(IServiceProvider provider)
        {
            _provider = provider;
            _client = provider.GetRequiredService<DiscordSocketClient>();

            _client.Ready += OnReadyAsync;
        }

        private Task OnReadyAsync()
        {
            new AutoUnmute(_provider);

            return _client.SetGameAsync(Configuration.GAME);
        }
    }
}
