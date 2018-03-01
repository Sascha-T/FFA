using Discord;
using Discord.Commands;
using Discord.Net;
using FFA.Common;
using FFA.Extensions.Discord;
using FFA.Extensions.System;
using System;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ResultService
    {
        private readonly LoggingService _logger;
        private readonly CommandService _commands;
        private readonly RateLimitService _rateLimitService;
        private readonly CustomCmdService _customCmdService;

        public ResultService(LoggingService logger, CommandService commands, RateLimitService rateLimitService, CustomCmdService customCmdService)
        {
            _logger = logger;
            _commands = commands;
            _rateLimitService = rateLimitService;
            _customCmdService = customCmdService;
        }

        public Task HandleResultAsync(Context context, IResult result, int argPos)
        {
            var message = result.ErrorReason;

            switch (result.Error)
            {
                // TODO: handle proper response for parse failed errors?
                case CommandError.UnknownCommand:
                    return _customCmdService.ExecuteAsync(context, argPos);
                case CommandError.Exception:
                    return HandleExceptionAsync(context, ((ExecuteResult)result).Exception);
                case CommandError.BadArgCount:
                    var cmd = _commands.GetCommand(context, argPos);

                    message = $"You are incorrectly using this command.\n" +
                              $"**Usage:** `{Config.PREFIX}{cmd.GetUsage()}`\n" +
                              $"**Example:** `{Config.PREFIX}{cmd.GetExample()}`";
                    break;
            }

            return context.ReplyErrorAsync(message);
        }

        public async Task HandleExceptionAsync(Context context, Exception exception)
        {
            var last = exception.Last();
            var message = last.Message;

            if (last is HttpException httpEx)
            {
                if ((int)httpEx.HttpCode == Config.TOO_MANY_REQUESTS)
                {
                    _rateLimitService.IgnoreUser(context.User.Id, Config.IGNORE_DURATION);
                    await context.DmAsync($"You will not be able to use commands for the next {Config.IGNORE_DURATION.TotalHours} hours." +
                        $"Please do not rate limit me.");
                    return;
                }
                else if (!Config.DISCORD_CODES.TryGetValue(httpEx.DiscordCode.GetValueOrDefault(), out message))
                {
                    Config.HTTP_CODES.TryGetValue(httpEx.HttpCode, out message);
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
