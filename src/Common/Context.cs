using Discord;
using Discord.Commands;
using FFA.Database;
using FFA.Database.Models;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Common
{
    public sealed class Context : ICommandContext
    {
        private readonly IServiceProvider _provider;
        private readonly SendingService _sender;

        public FFAContext Db { get; }
        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public IMessageChannel Channel { get; }
        public ITextChannel TextChannel { get; }
        public IUser User { get; }
        public IGuildUser GuildUser { get; }
        public IUserMessage Message { get; }
        public Guild DbGuild { get; private set; }
        public User DbUser { get; private set; }

        public Context(IDiscordClient client, IUserMessage msg, IServiceProvider provider)
        {
            _provider = provider;
            _sender = _provider.GetRequiredService<SendingService>();

            Db = _provider.GetRequiredService<FFAContext>();
            Client = client;
            Message = msg;
            Channel = msg.Channel;
            TextChannel = msg.Channel as ITextChannel;
            Guild = TextChannel?.Guild;
            User = msg.Author;
            GuildUser = User as IGuildUser;
        }

        public async Task InitializeAsync()
        {
            if (Guild != null)
            {
                DbUser = await Db.GetUserAsync(GuildUser);
                DbGuild = await Db.GetGuildAsync(Guild.Id);
            }
        }

        public async Task<IUserMessage> DmAsync(string description, string title = null)
            => await _sender.SendAsync(await User.GetOrCreateDMChannelAsync(), description, title, guild: Guild);

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
