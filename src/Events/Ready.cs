using Discord;
using FFA.Common;
using FFA.Entities.Event;
using FFA.Services;
using FFA.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class Ready : Event
    {
        private readonly IServiceProvider _provider;
        private readonly LoggingService _logger;

        public Ready(IServiceProvider provider) : base(provider)
        {
            _provider = provider;
            _logger = provider.GetRequiredService<LoggingService>();

            _client.Ready += OnReadyAsync;
        }

        private Task OnReadyAsync()
            => _taskService.TryRun(async () =>
            {
                Loader.LoadTimers(_provider);

                await _client.SetGameAsync(Config.GAME);
            });
    }
}
