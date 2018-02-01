using Discord;
using Discord.Commands;
using Discord.Net;
using System;
using System.Net;
using System.Threading.Tasks;
using FFA.Extensions;
using FFA.Services;

namespace FFA.Events
{
    class CommandLog
    {
        private readonly CommandService _commandService;
        private readonly Logger _logger;
        private readonly Sender _sender;

        public CommandLog(CommandService commandService, Logger logger, Sender sender)
        {
            _commandService = commandService;
            _logger = logger;
            _sender = sender;

            _commandService.Log += OnCommandLogAsync;
        }

        private Task OnCommandLogAsync(LogMessage msg)
        {
            if (msg.Exception is CommandException commandException)
            {
                var last = commandException.Last();

                if (last is HttpException discordException)
                {
                    var message = String.Empty;

                    switch (discordException.HttpCode)
                    {
                        case HttpStatusCode.Forbidden:
                            message = "I do not have permission to do that.";
                            break;
                        default:
                            message = last.Message;
                            break;
                    }

                    return _sender.ReplyErrorAsync(commandException.Context.User, commandException.Context.Channel, message);
                }
            }

            return _logger.LogAsync(msg.Severity, msg.Source + ": " + (msg.Exception?.ToString() ?? msg.Message));
        }
    }
}
