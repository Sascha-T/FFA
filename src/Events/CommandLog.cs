using Discord;
using Discord.Commands;
using Discord.Net;
using FFA.Extensions;
using FFA.Services;
using System.Net;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class CommandLog
    {
        private readonly CommandService _commandService;
        private readonly LoggingService _logger;
        private readonly SendingService _sender;

        public CommandLog(CommandService commandService, LoggingService logger, SendingService sender)
        {
            _commandService = commandService;
            _logger = logger;
            _sender = sender;

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
                    switch (discordException.HttpCode)
                    {
                        case HttpStatusCode.Forbidden:
                            message = "I do not have permission to do that.";
                            break;
                    }
                }

                await _sender.ReplyErrorAsync(commandException.Context.User, commandException.Context.Channel, message);
            }

            // TODO: some brilliant solution to not log on httpexception with nice boolean logic
            await _logger.LogAsync(msg.Severity, $"{msg.Source}: {(msg.Exception?.ToString() ?? msg.Message)}");
        }
    }
}
