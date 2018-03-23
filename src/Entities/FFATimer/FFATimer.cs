using Discord;
using Discord.WebSocket;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFA.Entities.FFATimer
{
    public abstract class FFATimer
    {
        protected readonly DiscordSocketClient _client;
        protected readonly LoggingService _logger;
        private readonly TaskService _taskService;
        private readonly Timer _timer;
        private readonly AutoResetEvent _state;

        public FFATimer(IServiceProvider provider, TimeSpan interval)
        {
            _client = provider.GetRequiredService<DiscordSocketClient>();
            _logger = provider.GetRequiredService<LoggingService>();
            _taskService = provider.GetRequiredService<TaskService>();
            _state = new AutoResetEvent(false);
            _timer = new Timer((state) =>
            {
                if (_client.ConnectionState != ConnectionState.Connected)
                    return;

                _taskService.TryRun(Execute);
            }, _state, TimeSpan.Zero, interval);
        }

        protected abstract Task Execute();
    }
}
