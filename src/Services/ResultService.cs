using Discord;
using Discord.Commands;
using Discord.Net;
using FFA.Common;
using FFA.Extensions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FFA.Services
{
    internal sealed class ResultService
    {
        private readonly LoggingService _logger;
        private readonly CommandService _commandService;
        
        internal ResultService(LoggingService logger, CommandService commandService)
        {
            _logger = logger;
            _commandService = commandService;
        }

        internal Task HandleResult(Context context, IResult result, int argPos)
        {
            var message = result.ErrorReason;

            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    return Task.CompletedTask;
                case CommandError.Exception:
                    return HandleException(context, ((ExecuteResult)result).Exception);
                case CommandError.BadArgCount:
                    var cmd = _commandService.GetCommand(context, argPos);

                    message = $"You are incorrectly using this command.\n" +
                              $"**Usage:** `{Configuration.PREFIX}{cmd.GetUsage()}`\n" +
                              $"**Example:** `{Configuration.PREFIX}{cmd.GetExample()}`";
                    break;
            }

            return context.ReplyErrorAsync(message);
        }

        internal async Task HandleException(Context context, Exception exception)
        {
            var last = exception.Last();
            var message = last.Message;

            if (last is HttpException discordException)
            {
                switch (discordException.HttpCode)
                {
                    case HttpStatusCode.Forbidden:
                        switch (discordException.DiscordCode)
                        {
                            case Configuration.CANNOT_DM_CODE:
                                message = "I cannot DM you. Please allow direct messages from guild users.";
                                break;
                            case Configuration.OLD_MSG_CODE:
                                message = "Discord does not allow bulk deletion of messages that are more than two weeks old.";
                                break;
                            case null:
                                message = "I do not have permission to do that.";
                                break;
                        }
                        break;
                }
            }
            else
            {
                await _logger.LogAsync(LogSeverity.Error, $"{exception}");
            }

            await context.ReplyErrorAsync(message);
        }
    }
}
