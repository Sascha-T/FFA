using Discord.WebSocket;
using FFA.Common;
using FFA.Timers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    internal sealed class Ready
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;

        internal Ready(IServiceProvider provider)
        {
            _provider = provider;
            _client = provider.GetRequiredService<DiscordSocketClient>();

            _client.Ready += OnReadyAsync;
        }

        private Task OnReadyAsync()
        {
            new AutoUnmute(_provider);

            return Task.Run(() => _client.SetGameAsync(Configuration.GAME));
        }
    }
}
