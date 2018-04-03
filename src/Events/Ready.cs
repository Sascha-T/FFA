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
        private bool _loadedTimers;

        public Ready(IServiceProvider provider) : base(provider)
        {
            _provider = provider;
            _logger = provider.GetRequiredService<LoggingService>();
            _loadedTimers = false;

            _client.Ready += OnReadyAsync;
        }

        private Task OnReadyAsync()
            => _taskService.TryRun(async () =>
            {
                if (!_loadedTimers)
                {
                    Loader.LoadTimers(_provider);
                    _loadedTimers = true;
                }

                await _client.SetGameAsync(Config.GAME);
            });
    }
}
