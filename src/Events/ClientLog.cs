using Discord;
using Discord.WebSocket;
using FFA.Services;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class ClientLog
    {
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _logger;

        // TODO: switch all events/timers to take in service provider in constructor
        public ClientLog(DiscordSocketClient client, LoggingService logger)
        {
            _client = client;
            _logger = logger;

            _client.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
            => _logger.LogAsync(msg.Severity, msg.Source + ": " + (msg.Exception?.ToString() ?? msg.Message));
    }
}
