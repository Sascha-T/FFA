using Discord;
using Discord.WebSocket;
using FFA.Common;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// TODO: common class for all timers, lots of code repetition...
namespace FFA.Timers
{
    public sealed class DlmBump
    {
        private readonly DiscordSocketClient _client;
        private readonly Credentials _credentials;
        private readonly LoggingService _logger;
        private readonly Timer _timer;
        private readonly AutoResetEvent _autoEvent;

        public DlmBump(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<LoggingService>();
            _credentials = provider.GetRequiredService<Credentials>();
            _client = provider.GetRequiredService<DiscordSocketClient>();
            _autoEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoEvent, 0, Config.DLM_BUMP_TIMER);
        }

        private void Execute(object state)
            => Task.Run(async () =>
            {
                try
                {
                    if (_client.ConnectionState != ConnectionState.Connected)
                        return;

                    var guild = _client.GetGuild(_credentials.GuildId);
                    var general = guild.TextChannels.FirstOrDefault(x => x.Name == "general");

                    if (general == default(SocketTextChannel))
                        return;

                    await general.SendMessageAsync("dlm!bump");
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(LogSeverity.Error, ex.ToString());
                }
            });
    }
}
