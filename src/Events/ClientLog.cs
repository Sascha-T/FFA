using Discord;
using Discord.WebSocket;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    internal sealed class ClientLog
    {
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _logger;
        
        internal ClientLog(IServiceProvider provider)
        {
            _client = provider.GetRequiredService<DiscordSocketClient>();
            _logger = provider.GetRequiredService<LoggingService>();

            _client.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
            => _logger.LogAsync(msg.Severity, msg.Source + ": " + (msg.Exception?.ToString() ?? msg.Message));
    }
}
