using Discord.Commands;
using Discord.WebSocket;
using FFA.Common;
using FFA.Extensions;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class MessageReceived
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly Configuration _config;
        private readonly IServiceProvider _provider;

        public MessageReceived(DiscordSocketClient client, CommandService commandService, Configuration config, IServiceProvider provider)
        {
            _client = client;
            _commandService = commandService;
            _config = config;
            _provider = provider;

            _client.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage socketMsg)
        {
            var msg = socketMsg as SocketUserMessage;

            if (msg == null)
            {
                return;
            }

            var context = new Context(_client, msg, _provider);

            int argPos = 0;

            if (msg.HasCharPrefix(_config.Prefix, ref argPos))
            {
                var result = await _commandService.ExecuteAsync(context, argPos, _provider);

                if (!result.IsSuccess)
                {
                    var message = string.Empty;

                    switch (result.Error.Value)
                    {
                        case CommandError.UnknownCommand:
                            // TODO: check for command similarity
                            return;
                        case CommandError.BadArgCount:
                            var cmd = _commandService.GetCommand(context, argPos);

                            message = $"You are incorrectly using this command.\n" +
                                      $"**Usage:** `{_config.Prefix}{cmd.GetUsage()}`\n" +
                                      $"**Example:** `{_config.Prefix}{cmd.GetExample()}`";
                            break;
                        default:
                            message = result.ErrorReason;
                            break;
                    }

                    await context.ReplyErrorAsync(message);
                }
            }
        }
    }
}
