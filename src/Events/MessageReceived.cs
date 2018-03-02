using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Entities.Event;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class MessageReceived : Event
    {
        private readonly CommandService _commandService;
        private readonly ResultService _resultService;
        private readonly SpamService _spamService;
        private readonly RateLimitService _rateLimitService;
        private readonly LoggingService _logger;
        private readonly IServiceProvider _provider;

        public MessageReceived(IServiceProvider provider) : base(provider)
        {
            _provider = provider;
            _commandService = _provider.GetRequiredService<CommandService>();
            _resultService = _provider.GetRequiredService<ResultService>();
            _spamService = _provider.GetRequiredService<SpamService>();
            _rateLimitService = _provider.GetRequiredService<RateLimitService>();
            _logger = _provider.GetRequiredService<LoggingService>();

            _client.MessageReceived += OnMessageReceivedAsync;
        }

        private Task OnMessageReceivedAsync(IMessage socketMsg)
            => _taskService.TryRun(async () =>
            {
                var msg = socketMsg as IUserMessage;

                if (msg == null || msg.Author.IsBot)
                    return;

                var context = new Context(_client, msg, _provider);

                await context.InitializeAsync();

                if (_rateLimitService.IsIgnored(context.User.Id))
                    return;
                // TODO: guild property to make auto spam detection optional
                else if (!await _spamService.AuthenticateAsync(context))
                    return;

                int argPos = 0;

                if (!msg.HasStringPrefix(Config.PREFIX, ref argPos))
                    return;

                var result = await _commandService.ExecuteAsync(context, argPos, _provider);

                if (!result.IsSuccess)
                    await _resultService.HandleResultAsync(context, result, argPos);
            });
    }
}
