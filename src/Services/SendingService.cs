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

        public SendingService(ThreadLocal<Random> random)
        {
            _random = random;
        }

        public async Task SendAsync(IMessageChannel channel, string description, string title = null, Color? color = null)
        {
            if (channel is ITextChannel textChannel && !await textChannel.CanSend())
            {
                return;
            }

            var builder = new EmbedBuilder
            {
                Color = color ?? _random.Value.ArrayElement(Configuration.Colors),
                Description = description,
                Title = title
            };

            await channel.SendMessageAsync("", false, builder.Build());
        }

        public Task ReplyAsync(IUser user, IMessageChannel channel, string description, string title = null, Color? color = null)
            => SendAsync(channel, $"{user.Bold()}, {description}", title, color);

        public Task ReplyErrorAsync(IUser user, IMessageChannel channel, string description)
            => ReplyAsync(user, channel, description, null, Configuration.ErrorColor);
    }
}
