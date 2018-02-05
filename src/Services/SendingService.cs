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

        public Task SendFieldsAsync(IMessageChannel channel, Color? color = null, params string[] fieldOrValue)
        {
            var builder = new EmbedBuilder
            {
                Color = color ?? _random.Value.ArrayElement(Configuration.DEFAULT_COLORS)
            };

            for (var i = 0; i < fieldOrValue.Length; i += 2)
            {
                builder.AddField(fieldOrValue[i], fieldOrValue[i + 1]);
            }

            return SendEmbedAsync(channel, builder);
        }

        public Task SendFieldsErrorAsync(IMessageChannel channel, params string[] fieldOrValue)
            => SendFieldsAsync(channel, Configuration.ERROR_COLOR, fieldOrValue);

        public Task SendAsync(IMessageChannel channel, string description, string title = null, Color? color = null)
        {
            return SendEmbedAsync(channel, new EmbedBuilder
            {
                Color = color ?? _random.Value.ArrayElement(Configuration.DEFAULT_COLORS),
                Description = description,
                Title = title
            });
        }

        public async Task SendEmbedAsync(IMessageChannel channel, EmbedBuilder builder)
        {
            if (channel is ITextChannel textChannel && !await textChannel.CanSend())
            {
                return;
            }

            await channel.SendMessageAsync("", false, builder.Build());
        }

        public Task ReplyAsync(IUser user, IMessageChannel channel, string description, string title = null, Color? color = null)
            => SendAsync(channel, $"{user.Bold()}, {description}", title, color);

        public Task ReplyErrorAsync(IUser user, IMessageChannel channel, string description)
            => ReplyAsync(user, channel, description, null, Configuration.ERROR_COLOR);
    }
}
