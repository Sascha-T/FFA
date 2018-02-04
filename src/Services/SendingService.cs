using Discord;
using FFA.Common;
using FFA.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class SendingService
    {
        private readonly ThreadLocal<Random> _random;
        private readonly Configuration _configuration;

        public SendingService(ThreadLocal<Random> random, Configuration configuration)
        {
            _random = random;
            _configuration = configuration;
        }

        public Task SendAsync(IMessageChannel channel, string description, string title = null, Color? color = null)
        {
            var builder = new EmbedBuilder
            {
                Color = color ?? _random.Value.ArrayElement(_configuration.Colors),
                Description = description,
                Title = title
            };

            return channel.SendMessageAsync("", false, builder.Build());
        }

        public Task ReplyAsync(IUser user, IMessageChannel channel, string description, string title = null, Color? color = null)
            => SendAsync(channel, $"{user.Bold()}, {description}", title, color);

        public Task ReplyErrorAsync(IUser user, IMessageChannel channel, string description)
            => ReplyAsync(user, channel, description, null, _configuration.ErrorColor);
    }
}
