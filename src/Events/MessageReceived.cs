using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Entities.Event;
using FFA.Extensions.Discord;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class MessageReceived : Event
    {
        private readonly CommandService _commandService;
        private readonly ChatService _chatService;
        private readonly ResultService _resultService;
        private readonly SpamService _spamService;
        private readonly RateLimitService _rateLimitService;
        private readonly LoggingService _logger;
        private readonly IServiceProvider _provider;

        public MessageReceived(IServiceProvider provider) : base(provider)
        {
            _provider = provider;
            _commandService = _provider.GetRequiredService<CommandService>();
            _chatService = _provider.GetRequiredService<ChatService>();
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

                var ctx = new Context(_client, msg, _provider);

                if (ctx.Channel is ITextChannel textChannel && !await textChannel.CanSendAsync())
                    return;

                await ctx.InitializeAsync();

                if (_rateLimitService.IsIgnored(ctx.User.Id))
                    return;
                else if (ctx.Guild != null && ctx.DbGuild.AutoMute && !await _spamService.AuthenticateAsync(ctx))
                    return;

                int argPos = 0;

                if (!msg.HasStringPrefix(Config.PREFIX, ref argPos))
                {
                    await _chatService.ApplyAsync(ctx);
                    return;
                }

                var result = await _commandService.ExecuteAsync(ctx, argPos, _provider);

                await _resultService.HandleResultAsync(ctx, result, argPos);
            });
    }
}
