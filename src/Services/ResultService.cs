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
        private readonly CommandService _commandService;
        private readonly RateLimitService _rateLimitService;
        private readonly CustomCommandService _customCommandService;

        public ResultService(LoggingService logger, CommandService commandService, RateLimitService rateLimitService,
            CustomCommandService customCommandService)
        {
            _logger = logger;
            _commandService = commandService;
            _rateLimitService = rateLimitService;
            _customCommandService = customCommandService;
        }

        public Task HandleResultAsync(Context context, IResult result, int argPos)
        {
            var message = result.ErrorReason;

            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    return _customCommandService.ExecuteAsync(context, argPos);
                case CommandError.Exception:
                    return HandleExceptionAsync(context, ((ExecuteResult)result).Exception);
                case CommandError.BadArgCount:
                    var cmd = _commandService.GetCommand(context, argPos);

                    message = $"You are incorrectly using this command.\n" +
                              $"**Usage:** `{Configuration.PREFIX}{cmd.GetUsage()}`\n" +
                              $"**Example:** `{Configuration.PREFIX}{cmd.GetExample()}`";
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
                if ((int)httpEx.HttpCode == Configuration.TOO_MANY_REQUESTS)
                {
                    _rateLimitService.IgnoreUser(context.User.Id, Configuration.IGNORE_DURATION);
                    await context.DmAsync($"You will not be able to use commands for the next {Configuration.IGNORE_DURATION.TotalHours} hours." +
                        $"Please do not rate limit me.");
                    return;
                }
                else if (!Configuration.DISCORD_CODE_RESPONSES.TryGetValue(httpEx.DiscordCode.GetValueOrDefault(), out message))
                {
                    Configuration.HTTP_CODE_RESPONSES.TryGetValue(httpEx.HttpCode, out message);
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
