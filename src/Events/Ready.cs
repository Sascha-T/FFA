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
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _provider;

        public Ready(DiscordSocketClient client, IServiceProvider provider)
        {
            _client = client;
            _provider = provider;

            _client.Ready += OnReadyAsync;
        }

        private Task OnReadyAsync()
        {
            _provider.GetRequiredService<AutoUnmute>();

            return _client.SetGameAsync(Configuration.GAME);
        }
    }
}
