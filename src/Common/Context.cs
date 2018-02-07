using Discord;
using Discord.Commands;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Common
{
    public class Context : ICommandContext
    {
        private readonly IServiceProvider _provider;
        private readonly SendingService _sender;

        public IDiscordClient Client { get; private set; }
        public IGuild Guild { get; private set; }
        public IMessageChannel Channel { get; private set; }
        public ITextChannel TextChannel { get; private set; }
        public IUser User { get; private set; }
        public IGuildUser GuildUser { get { return User as IGuildUser; } }
        public IUserMessage Message { get; private set; }

        public Context(IDiscordClient client, IUserMessage msg, IServiceProvider provider)
        {
            _provider = provider;
            _sender = _provider.GetRequiredService<SendingService>();

            Client = client;
            Message = msg;
            Channel = msg.Channel;
            TextChannel = msg.Channel as ITextChannel;
            Guild = TextChannel?.Guild;
            User = msg.Author;
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
