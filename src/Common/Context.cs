using Discord;
using Discord.Commands;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Common
{
    public sealed class Context : ICommandContext
    {
        private readonly SendingService _sender;
        private readonly IMongoCollection<User> _dbUsers;
        private readonly IMongoCollection<Guild> _dbGuilds;

        public User DbUser { get; private set; }
        public Guild DbGuild { get; private set; }
        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public IMessageChannel Channel { get; }
        public ITextChannel TextChannel { get; }
        public IUser User { get; }
        public IGuildUser GuildUser { get; }
        public IUserMessage Message { get; }

        public Context(IDiscordClient client, IUserMessage msg, IServiceProvider provider)
        {
            _sender = provider.GetRequiredService<SendingService>();
            _dbUsers = provider.GetRequiredService<IMongoCollection<User>>();
            _dbGuilds = provider.GetRequiredService<IMongoCollection<Guild>>();

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
                DbUser = await _dbUsers.GetUserAsync(GuildUser);
                DbGuild = await _dbGuilds.GetGuildAsync(Guild.Id);
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
