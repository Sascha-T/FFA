using Discord;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using System;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ModerationService
    {
        public Task LogMuteAsync(Context context, IUser subject, Rule rule, TimeSpan length, string reason = null)
        {
            var description = $"**Action:** Mute\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              $"**Rule:** {rule.Content}\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}\n") +
                              $"**Length:** {length.TotalHours}h";

            return LogAsync(context.Db, context.Guild, description, Configuration.MUTE_COLOR, context.User);
        }

        public Task LogAutoMuteAsync(Context context, TimeSpan length)
        {
            var description = $"**Action:** Automatic Mute\n" +
                              $"**User:** {context.User} ({context.User.Id})\n" +
                              $"**Reason:** Sending {Configuration.SPAM_LIMIT} or more similar messges in " +
                              $"{Configuration.SPAM_DURATION.TotalSeconds} seconds or less.\n" +
                              $"**Length:** {length.TotalHours}h";

            return LogAsync(context.Db, context.Guild, description, Configuration.MUTE_COLOR);
        }

        public Task LogUnmuteAsync(Context context, IUser subject, string reason = null)
        {
            var description = $"**Action:** Unmute\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}");

            return LogAsync(context.Db, context.Guild, description, Configuration.UNMUTE_COLOR, context.User);
        }

        public Task LogAutoUnmuteAsync(FFAContext ffaContext, IGuild guild, IUser subject)
            => LogAsync(ffaContext, guild, $"**Action:** Automatic Unmute\n**User:** {subject} ({subject.Id})\n", Configuration.UNMUTE_COLOR);

        public Task LogClearAsync(Context context, IUser subject, Rule rule, int quantity, string reason = null)
        {
            var description = $"**Action:** Clear\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              $"**Rule:** {rule.Content}\n" +
                              $"**Quantity:** {quantity}\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}\n");

            return LogAsync(context.Db, context.Guild, description, Configuration.CLEAR_COLOR, context.User);
        }

        public async Task LogAsync(FFAContext ffaContext, IGuild guild, string description, Color color, IUser moderator = null)
        {
            var dbGuild = await ffaContext.GetGuildAsync(guild.Id);

            if (!dbGuild.LogChannelId.HasValue)
            {
                return;
            }

            var logChannel = await guild.GetChannelAsync(dbGuild.LogChannelId.Value) as ITextChannel;

            if (logChannel == null || !await logChannel.CanSendAsync())
            {
                return;
            }

            var builder = new EmbedBuilder()
            {
                Timestamp = DateTimeOffset.Now,
                Footer = new EmbedFooterBuilder { Text = $"Case #{dbGuild.LogCase}" },
                Description = description,
                Color = color
            };

            if (moderator != null)
            {
                builder.WithAuthor(new EmbedAuthorBuilder
                {
                    Name = $"{moderator} ({moderator.Id})",
                    IconUrl = moderator.GetAvatarUrl()
                });
            }

            await logChannel.SendMessageAsync("", false, builder.Build());
            await ffaContext.UpdateAsync(dbGuild, x => x.LogCase++);
        }
    }
}
