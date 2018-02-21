using Discord;
using Discord.Commands;
using Discord.Net;
using FFA.Common;
using FFA.Extensions;
using System;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ResultService
    {
        private readonly LoggingService _logger;
        private readonly CommandService _commandService;

        public ResultService(LoggingService logger, CommandService commandService)
        {
            _logger = logger;
            _commandService = commandService;
        }

        public Task HandleResult(Context context, IResult result, int argPos)
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

        public async Task HandleException(Context context, Exception exception)
        {
            var last = exception.Last();
            var message = last.Message;

            if (last is HttpException httpEx)
            {
                if (!Configuration.DISCORD_CODE_RESPONSES.TryGetValue(httpEx.DiscordCode.GetValueOrDefault(), out message))
                    Configuration.HTTP_CODE_RESPONSES.TryGetValue(httpEx.HttpCode, out message);
            }
            else
            {
                await _logger.LogAsync(LogSeverity.Error, $"{exception}");
            }

            await context.ReplyErrorAsync(message);
        }
    }
}
