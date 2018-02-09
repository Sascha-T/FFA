using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FFA.Common;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    internal sealed class MessageReceived
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly ResultService _resultService;
        private readonly IServiceProvider _provider;

        internal MessageReceived(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetRequiredService<DiscordSocketClient>();
            _commandService = _provider.GetRequiredService<CommandService>();
            _resultService = _provider.GetRequiredService<ResultService>();

            _client.MessageReceived += OnMessageReceivedAsync;
        }

        private Task OnMessageReceivedAsync(IMessage socketMsg)
        {
            Task.Run(async () =>
            {
                var msg = socketMsg as IUserMessage;

                if (msg == null || msg.Author.IsBot)
                {
                    return;
                }

                var context = new Context(_client, msg, _provider);

                await context.InitializeAsync();

                int argPos = 0;

                if (msg.HasStringPrefix(Configuration.PREFIX, ref argPos))
                {
                    var result = await _commandService.ExecuteAsync(context, argPos, _provider);

                    if (!result.IsSuccess)
                    {
                        await _resultService.HandleResult(context, result, argPos);
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
}
