using Discord.Commands;
using Discord.WebSocket;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FFA.Services;
using Discord;

namespace FFA.Common
{
    public class Context : SocketCommandContext
    {
        private readonly IServiceProvider _provider;
        private readonly Sender _sender;

        public Context(DiscordSocketClient client, SocketUserMessage msg, IServiceProvider provider) : base(client, msg)
        {
            _provider = provider;
            _sender = _provider.GetRequiredService<Sender>();
        }

        public Task SendAsync(string description, string title = null, Color? color = null)
        {
            return _sender.SendAsync(Channel, description, title, color);
        }

        public Task ReplyAsync(string description, string title = null, Color? color = null)
        {
            return _sender.ReplyAsync(User, Channel, description, title, color);
        }
    }
}
