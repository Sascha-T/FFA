using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Common
{
    public class Context : SocketCommandContext
    {
        private readonly IServiceProvider _provider;
        private readonly SendingService _sender;

        public Context(DiscordSocketClient client, SocketUserMessage msg, IServiceProvider provider) : base(client, msg)
        {
            _provider = provider;
            _sender = _provider.GetRequiredService<SendingService>();
        }

        public async Task<IUserMessage> DmAsync(string description, string title = null)
            => await _sender.SendAsync(await User.GetOrCreateDMChannelAsync(), description, title);

        public Task<IUserMessage> SendFieldsAsync(Color? color = null, params string[] fieldOrValue)
            => _sender.SendFieldsAsync(Channel, color, fieldOrValue);

        public Task<IUserMessage> SendFieldsErrorAsync(params string[] fieldOrValue)
            => _sender.SendFieldsErrorAsync(Channel, fieldOrValue);

        public Task<IUserMessage> SendAsync(string description, string title = null, Color? color = null)
            => _sender.SendAsync(Channel, description, title, color);

        public Task<IUserMessage> ReplyAsync(string description, string title = null, Color? color = null)
            => _sender.ReplyAsync(User, Channel, description, title, color);

        public Task<IUserMessage> ReplyErrorAsync(string description)
            => _sender.ReplyErrorAsync(User, Channel, description);
    }
}
