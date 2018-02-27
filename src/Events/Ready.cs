using Discord;
using Discord.WebSocket;
using FFA.Common;
using FFA.Services;
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
        private readonly LoggingService _logger;

        internal Ready(IServiceProvider provider)
        {
            _provider = provider;
            _client = provider.GetRequiredService<DiscordSocketClient>();
            _logger = provider.GetRequiredService<LoggingService>();

            _client.Ready += OnReadyAsync;
        }

        private Task OnReadyAsync()
        {
            Task.Run(async () =>
            {
                try
                {
                    new AutoUnmute(_provider);

                    await _client.SetGameAsync(Configuration.GAME);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(LogSeverity.Error, ex.ToString());
                }
            });

            return Task.CompletedTask;
        }
    }
}
