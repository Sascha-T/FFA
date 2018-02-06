using Discord;
using Discord.Commands;
using Discord.Net;
using FFA.Extensions;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FFA.Events
{
    internal sealed class CommandLog
    {
        private readonly CommandService _commandService;
        private readonly LoggingService _logger;
        private readonly SendingService _sender;

        internal CommandLog(IServiceProvider provider)
        {
            _commandService = provider.GetRequiredService<CommandService>();
            _logger = provider.GetRequiredService<LoggingService>();
            _sender = provider.GetRequiredService<SendingService>();

            _commandService.Log += OnCommandLogAsync;
        }

        private async Task OnCommandLogAsync(LogMessage msg)
        {
            if (msg.Exception is CommandException commandException)
            {
                var last = commandException.Last();
                var message = last.Message;

                if (last is HttpException discordException)
                {
                    if (discordException.DiscordCode.HasValue)
                    {
                        switch (discordException.DiscordCode.Value)
                        {
                            case 50017:
                                message = "I cannot DM you. Please allow direct messages from guild members.";
                                break;
                        }
                    }
                    else
                    {
                        switch (discordException.HttpCode)
                        {
                            case HttpStatusCode.Forbidden:
                                message = "I do not have permission to do that.";
                                break;
                        }
                    }
                }
                else
                {
                    await _logger.LogAsync(msg.Severity, $"{msg.Source}: {(msg.Exception?.ToString() ?? msg.Message)}");
                }

                _ = Task.Run(() => _sender.ReplyErrorAsync(commandException.Context.User, commandException.Context.Channel, message));
            }
        }
    }
}
